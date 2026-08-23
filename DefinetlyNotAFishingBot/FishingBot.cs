using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace DefinetlyNotAFishingBot {

  /// <summary>Coarse bot state, used by the UI to color the window background.</summary>
  internal enum BotState {
    Stopped,
    Running,
    Paused
  }

  /// <summary>What the bot is currently doing, used to color the capture overlay's frame.</summary>
  internal enum BotPhase {
    /// <summary>Bot is not running — the overlay frame is red.</summary>
    Off,
    /// <summary>Casting/watching for the bite — the overlay frame is blue.</summary>
    Fishing,
    /// <summary>A fish bit and is being looted — the overlay frame is green.</summary>
    Looting
  }

  /// <summary>
  /// The actual bot: runs the cast → watch → loot loop on a background thread.
  /// All game interaction is purely pixel and input based (screen capture plus
  /// SendInput), no memory reading and no injection, so the bot needs no
  /// administrator rights as long as WoW itself is not started elevated.
  ///
  /// Start sequence: equip the fishing outfit (which includes the pole), apply
  /// the lure onto the equipped pole, apply the buff — each followed by a wait
  /// so the apply/cast time and the server round trip complete before going on.
  ///
  /// Loop: wait the refish time, cast, watch for the bite; on a bite move the
  /// mouse onto the bobber right away, wait the loot time, right-click to loot,
  /// wait for the server, then re-apply lure/buff when their timers say they
  /// are about to run out (the real buffs cannot be read without touching game
  /// memory, so timers are the best available signal).
  ///
  /// The loop pauses automatically whenever the WoW window loses focus, so the
  /// developer can take over at any time by alt-tabbing; giving WoW the focus
  /// again resumes the bot.
  /// </summary>
  internal class FishingBot {

    /// <summary>Lures last 10 minutes; re-apply a bit early so the buff never runs out.</summary>
    private const int LURE_INTERVAL_MS = 9 * 60 * 1000;
    /// <summary>Assumed buff duration; re-apply on this timer (adjust to the buff in use).</summary>
    private const int BUFF_INTERVAL_MS = 9 * 60 * 1000;
    /// <summary>Applying a lure channels for 5 seconds; wait a little longer to be safe.</summary>
    private const int LURE_APPLY_WAIT_MS = 6500;
    /// <summary>Wait after the buff key so its cast/GCD and the server round trip complete.</summary>
    private const int BUFF_APPLY_WAIT_MS = 3000;
    /// <summary>Wait after the outfit key so the gear swap really is on the server before the lure.</summary>
    private const int OUTFIT_APPLY_WAIT_MS = 4000;
    /// <summary>Time after pressing the fishing key until the bobber has landed and the water has calmed down.</summary>
    private const int CAST_SETTLE_MS = 2500;
    /// <summary>In 3.3.5 a fish bites within ~17 s of the bobber landing; give up and recast after this long.</summary>
    private const int WATCH_TIMEOUT_MS = 25000;
    /// <summary>No fish ever bites this early after the bobber landed — ignore bite signals before then.</summary>
    private const int MIN_WATCH_BEFORE_BITE_MS = 2000;
    /// <summary>
    /// Pause between bobber scans while waiting for the bite. The dip only
    /// lasts a few hundred milliseconds, so the watcher runs at ~25-30 scans
    /// per second — feasible because it only captures the small watch region.
    /// </summary>
    private const int SAMPLE_INTERVAL_MS = 30;
    /// <summary>Number of initial scans averaged into the "calm bobber" baseline.</summary>
    private const int BASELINE_SAMPLES = 5;
    /// <summary>Minimum matching pixels for the bobber to count as "found".</summary>
    private const int MIN_BOBBER_PIXELS = 12;
    /// <summary>The splash swallows the bobber: it is a bite when the match count drops below this fraction of the baseline.</summary>
    private const double BITE_COUNT_FACTOR = 0.5;
    /// <summary>Or the bobber visibly dips: it is a bite when its center moves down this many pixels.</summary>
    private const int BITE_DIP_PIXELS = 8;
    /// <summary>Half size of the square region around the found bobber that is watched for the bite.</summary>
    private const int WATCH_RADIUS = 40;
    /// <summary>Wait after the loot click so the loot reaches the bags before anything else happens.</summary>
    private const int POST_LOOT_WAIT_MS = 1500;

    private readonly ScreenManager screenManager;
    private readonly WowWindow wowWindow = new WowWindow();

    private Thread botThread = null;
    private volatile bool stopRequested = false;
    private DateTime lastLureTime = DateTime.MinValue;
    private DateTime lastBuffTime = DateTime.MinValue;
    private int fishCaught = 0;
    private bool uiHidden = false;

    private readonly object markerLock = new object();
    private Rectangle bobberMarker = Rectangle.Empty;

    /// <summary>
    /// The region (in capture coordinates) the bot currently believes the bobber
    /// is in; Rectangle.Empty while no bobber is being watched. The preview in
    /// frmMain draws this so the developer can see what the bot is looking at.
    /// </summary>
    internal Rectangle BobberMarker {
      get { lock (markerLock) { return bobberMarker; } }
    }

    private void SetBobberMarker(Rectangle region) {
      lock (markerLock) { bobberMarker = region; }
    }

    /// <summary>Raised from the bot thread whenever there is a new status message to show.</summary>
    internal event Action<string> StatusChanged;
    /// <summary>Raised from the bot thread whenever the coarse state (running/paused/stopped) changes.</summary>
    internal event Action<BotState> StateChanged;
    /// <summary>Raised from the bot thread whenever the activity phase (off/fishing/looting) changes.</summary>
    internal event Action<BotPhase> PhaseChanged;
    /// <summary>Raised from the bot thread whenever a fish was looted, with the total of this run.</summary>
    internal event Action<int> CaughtChanged;
    /// <summary>Raised from the bot thread when the loop has terminated for any reason.</summary>
    internal event Action Stopped;

    /// <summary>True while the bot thread is alive.</summary>
    internal bool IsRunning {
      get { return botThread != null && botThread.IsAlive; }
    }

    /// <param name="screenManager">Provides the capture region defined by the overlay window.</param>
    internal FishingBot(ScreenManager screenManager) {
      this.screenManager = screenManager;
    }

    /// <summary>Starts the bot loop on a background thread. Does nothing if it is already running.</summary>
    internal void Start() {
      if (IsRunning)
        return;
      stopRequested = false;
      fishCaught = 0;
      botThread = new Thread(RunLoop) { IsBackground = true, Name = "FishingBot" };
      botThread.Start();
    }

    /// <summary>Signals the bot loop to stop and waits briefly for the thread to end.</summary>
    internal void Stop() {
      stopRequested = true;
      if (botThread != null && botThread.IsAlive)
        botThread.Join(3000);
    }

    /// <summary>The complete bot life cycle: find WoW, prepare once, then fish until stopped.</summary>
    private void RunLoop() {
      try {
        if (!wowWindow.Find()) {
          Status("WoW window not found — is the game running?");
          return;
        }

        ChangeState(BotState.Running);
        ChangePhase(BotPhase.Fishing);
        Status("Bringing WoW to the foreground…");
        if (!wowWindow.EnsureForeground()) {
          Status("Could not focus WoW — click into the game window…");
          if (!WaitForWowFocus())
            return;
        }
        SleepChecked(1000);

        // Hide the game UI (Alt+Y) so no interface element can cover the bobber
        // or disturb the pixel detection; it is restored when the bot stops.
        // The developer can switch this off via the checkbox in the settings.
        if (Config.HideGameUi)
          ToggleGameUi("Hiding the game UI");

        // Start sequence — order matters: the outfit equips the fishing pole,
        // only then can the lure be applied onto it. Keys set to None are skipped.
        PressAndWait(Config.OutfitKey, "Equipping fishing outfit", OUTFIT_APPLY_WAIT_MS);
        PressAndWait(Config.LureKey, "Applying lure", LURE_APPLY_WAIT_MS);
        lastLureTime = DateTime.UtcNow;
        PressAndWait(Config.BuffKey, "Applying buff", BUFF_APPLY_WAIT_MS);
        lastBuffTime = DateTime.UtcNow;

        while (!stopRequested) {
          SetBobberMarker(Rectangle.Empty);

          Status(CaughtPrefix() + "Waiting to refish…");
          SleepChecked(Config.RefishTime * 1000);
          if (stopRequested)
            break;
          if (!WaitForWowFocus())
            break;

          // Park the mouse outside the capture area first: if the bobber lands
          // under the cursor, the mouse-over glow brightens it and the color
          // detection fails.
          MoveMouseAside();

          Status(CaughtPrefix() + "Casting… [key: " + Config.FishingKey + "]");
          if (!wowWindow.IsForeground && !WaitForWowFocus())
            break;
          InputSender.PressKey(Config.FishingKey);
          SleepChecked(CAST_SETTLE_MS);
          if (stopRequested)
            break;

          BobberSample bobber = ScanForBobber();
          if (bobber.MatchCount < MIN_BOBBER_PIXELS) {
            Status(CaughtPrefix() + "Bobber not found (" + bobber.MatchCount + " px matched) — recasting…");
            continue; // The loop restarts with the refish wait.
          }

          if (WatchForBite(bobber, out Point clickPoint)) {
            // Move onto the bobber right away, then give the loot timer time to
            // pass (the client registers the hover meanwhile), then loot.
            ChangePhase(BotPhase.Looting);
            Status(CaughtPrefix() + "Bite! Moving onto the bobber…");
            MoveToBobber(clickPoint);
            SleepChecked(Config.LootTime * 1000);
            if (stopRequested)
              break;
            if (!wowWindow.IsForeground) {
              ChangePhase(BotPhase.Fishing);
              continue; // Focus was taken over mid-loot — skip the click.
            }
            InputSender.RightClick();
            fishCaught++;
            CaughtChanged?.Invoke(fishCaught);
            Status(CaughtPrefix() + "Looted — waiting for the server…");
            SleepChecked(POST_LOOT_WAIT_MS);
            ChangePhase(BotPhase.Fishing);
          } else if (!stopRequested) {
            Status(CaughtPrefix() + "No bite — recasting…");
          }

          // Keep lure and buff alive before the next cast.
          if (!stopRequested && (DateTime.UtcNow - lastLureTime).TotalMilliseconds >= LURE_INTERVAL_MS) {
            PressAndWait(Config.LureKey, CaughtPrefix() + "Re-applying lure", LURE_APPLY_WAIT_MS);
            lastLureTime = DateTime.UtcNow;
          }
          if (!stopRequested && (DateTime.UtcNow - lastBuffTime).TotalMilliseconds >= BUFF_INTERVAL_MS) {
            PressAndWait(Config.BuffKey, CaughtPrefix() + "Re-applying buff", BUFF_APPLY_WAIT_MS);
            lastBuffTime = DateTime.UtcNow;
          }
        }

        Status("Stopped.");
      } catch (Exception ex) {
        Status("Error: " + ex.Message);
      } finally {
        // Bring the game UI back; skipped (with a hint) when WoW lost the
        // focus meanwhile, so the combo cannot land in another window.
        if (uiHidden) {
          if (wowWindow.IsForeground)
            ToggleGameUi("Restoring the game UI");
          else
            Status("Game UI is still hidden — press Alt+Y in WoW to bring it back.");
        }
        SetBobberMarker(Rectangle.Empty);
        ChangePhase(BotPhase.Off);
        ChangeState(BotState.Stopped);
        Stopped?.Invoke();
      }
    }

    /// <summary>Captures the overlay region once and searches the whole image for the bobber.</summary>
    private BobberSample ScanForBobber() {
      using (Bitmap shot = screenManager.GetScreenCapture()) {
        return BobberDetector.FindBobber(shot, Config.BobberColor, Config.ColorTollerance);
      }
    }

    /// <summary>
    /// Watches a small region around the found bobber until the bite is detected
    /// (match count collapses into the splash, or the bobber center dips down),
    /// the timeout expires, the WoW window loses focus, or a stop is requested.
    /// Returns true only for a detected bite; clickPoint is the last position the
    /// bobber was clearly seen at (it drifts slightly while waiting), in capture
    /// coordinates.
    /// </summary>
    private bool WatchForBite(BobberSample initial, out Point clickPoint) {
      clickPoint = initial.Centroid;
      Rectangle captureArea = new Rectangle(Point.Empty, screenManager.GetCaptureRectangle().Size);
      Rectangle watchRegion = Rectangle.Intersect(
        new Rectangle(
          initial.Centroid.X - WATCH_RADIUS,
          initial.Centroid.Y - WATCH_RADIUS,
          WATCH_RADIUS * 2,
          WATCH_RADIUS * 2
        ),
        captureArea
      );
      if (watchRegion.Width <= 0 || watchRegion.Height <= 0)
        return false;

      SetBobberMarker(watchRegion);

      int samples = 0;
      int countSum = 0;
      int baselineCount = 0;
      int baselineY = initial.Centroid.Y;
      int biteStreak = 0;
      DateTime watchStart = DateTime.UtcNow;

      Status(CaughtPrefix() + "Bobber at (" + initial.Centroid.X + ", " + initial.Centroid.Y + "), "
        + initial.MatchCount + " px — watching…");

      while (!stopRequested && (DateTime.UtcNow - watchStart).TotalMilliseconds < WATCH_TIMEOUT_MS) {
        if (!wowWindow.IsForeground)
          return false;

        // Capture only the small watch region — a full-overlay capture per
        // sample would throttle the loop to a few frames per second.
        BobberSample sample;
        using (Bitmap shot = screenManager.GetScreenCapture(watchRegion)) {
          sample = BobberDetector.Scan(shot, new Rectangle(Point.Empty, shot.Size), Config.BobberColor, Config.ColorTollerance);
        }
        if (sample.MatchCount > 0)
          sample.Centroid = new Point(sample.Centroid.X + watchRegion.X, sample.Centroid.Y + watchRegion.Y);

        samples++;
        if (sample.MatchCount >= MIN_BOBBER_PIXELS)
          clickPoint = sample.Centroid;
        if (samples <= BASELINE_SAMPLES) {
          // Build the "calm bobber" baseline from the first few scans.
          countSum += sample.MatchCount;
          baselineCount = countSum / samples;
          if (sample.MatchCount > 0)
            baselineY = sample.Centroid.Y;
          if (samples == BASELINE_SAMPLES) {
            // Sanity check: when a large part of the watch region matches, the
            // picked color matches the environment and every ripple would look
            // like a bite — better to tell the developer than to click wildly.
            int regionArea = watchRegion.Width * watchRegion.Height;
            if (baselineCount > regionArea / 2) {
              Status(CaughtPrefix() + "Color matches too much water (" + baselineCount
                + " px) — lower the tolerance or re-pick the bobber color!");
              return false;
            }
            Status(CaughtPrefix() + "Watching… (baseline " + baselineCount + " px)");
          }
        } else {
          if (baselineCount < MIN_BOBBER_PIXELS)
            return false; // The bobber was too faint to watch reliably.
          bool biteSignal = sample.MatchCount < baselineCount * BITE_COUNT_FACTOR
            || (sample.MatchCount > 0 && sample.Centroid.Y - baselineY >= BITE_DIP_PIXELS);
          // Two consecutive samples must agree, so single-frame noise (ripples,
          // light flicker) cannot fake a bite — and no signal counts during the
          // first seconds, because no fish bites right after the cast.
          biteStreak = biteSignal ? biteStreak + 1 : 0;
          if (biteStreak >= 2 && (DateTime.UtcNow - watchStart).TotalMilliseconds >= MIN_WATCH_BEFORE_BITE_MS)
            return true;
        }

        SleepChecked(SAMPLE_INTERVAL_MS);
      }

      return false;
    }

    /// <summary>
    /// Parks the cursor just right of the capture area (or left, when the right
    /// side would leave the screen), so no mouse-over glow can touch the bobber.
    /// </summary>
    private void MoveMouseAside() {
      Rectangle captureRect = screenManager.GetCaptureRectangle();
      Rectangle screen = SystemInformation.VirtualScreen;
      int x = captureRect.Right + 40;
      if (x > screen.Right - 2)
        x = Math.Max(screen.Left + 2, captureRect.Left - 40);
      int y = captureRect.Y + captureRect.Height / 2;
      InputSender.MoveCursor(new Point(x, y));
      SleepChecked(150);
    }

    /// <summary>Moves the cursor onto the bobber (capture coordinates mapped to the screen).</summary>
    private void MoveToBobber(Point captureCoordinate) {
      Rectangle captureRect = screenManager.GetCaptureRectangle();
      Point screenPoint = new Point(captureRect.X + captureCoordinate.X, captureRect.Y + captureCoordinate.Y);
      InputSender.MoveCursor(screenPoint);
    }

    /// <summary>
    /// Presses one of the utility keys and waits; keys set to None are skipped
    /// entirely. Blocks until WoW really has the focus — re-checked immediately
    /// before the press — so the key press cannot land in another window.
    /// </summary>
    private void PressAndWait(Keys key, string statusText, int waitMs) {
      if (key == Keys.None || stopRequested)
        return;
      if (!WaitForWowFocus())
        return;
      Status(statusText + "… [key: " + key + ", then waiting " + (waitMs / 1000.0).ToString("0.#") + "s]");
      if (!wowWindow.IsForeground && !WaitForWowFocus())
        return;
      InputSender.PressKey(key);
      SleepChecked(waitMs);
    }

    /// <summary>
    /// Toggles the WoW interface with Alt+Y so nothing covers the bobber; only
    /// sent while WoW has the focus. Tracks the toggle so the UI is restored
    /// exactly once when the bot stops.
    /// </summary>
    private void ToggleGameUi(string statusText) {
      if (!wowWindow.IsForeground)
        return;
      Status(statusText + "… [Alt+Y]");
      InputSender.PressAltCombo(Keys.Y);
      Thread.Sleep(400);
      uiHidden = !uiHidden;
    }

    /// <summary>
    /// Blocks while the WoW window does not have the focus, so the developer can
    /// take over at any time. Returns false when a stop was requested meanwhile.
    /// </summary>
    private bool WaitForWowFocus() {
      if (wowWindow.IsForeground)
        return true;
      ChangeState(BotState.Paused);
      Status(CaughtPrefix() + "Paused — click into the WoW window to continue…");
      while (!stopRequested && !wowWindow.IsForeground)
        Thread.Sleep(250);
      if (stopRequested)
        return false;
      ChangeState(BotState.Running);
      Status(CaughtPrefix() + "WoW focused again — resuming…");
      SleepChecked(750);
      return true;
    }

    /// <summary>Sleeps in small slices so a stop request is honored quickly.</summary>
    private void SleepChecked(int milliseconds) {
      const int SLICE_MS = 50;
      int remaining = milliseconds;
      while (remaining > 0 && !stopRequested) {
        int slice = Math.Min(SLICE_MS, remaining);
        Thread.Sleep(slice);
        remaining -= slice;
      }
    }

    /// <summary>Status prefix showing the catch counter once the first fish is in the bag.</summary>
    private string CaughtPrefix() {
      return fishCaught > 0 ? "Caught " + fishCaught + " | " : "";
    }

    /// <summary>Raises the status event; the UI marshals it onto its own thread.</summary>
    private void Status(string text) {
      StatusChanged?.Invoke(text);
    }

    /// <summary>Raises the state event; the UI marshals it onto its own thread.</summary>
    private void ChangeState(BotState state) {
      StateChanged?.Invoke(state);
    }

    /// <summary>Raises the phase event; the UI marshals it onto its own thread.</summary>
    private void ChangePhase(BotPhase phase) {
      PhaseChanged?.Invoke(phase);
    }
  }
}
