# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

A Windows Forms fishing bot for WoW WotLK 3.3.5a (used on a private server where bots are permitted): the developer positions a transparent overlay window over the water, the app captures that screen region, finds the bobber by its picked color, detects the bite, and right-clicks to loot — purely pixel- and input-based, no memory reading or injection, and it must never require administrator rights (SendInput reaches WoW as long as the game itself is not elevated). Classic .NET Framework 4.8 project (`packages.config` style, not SDK-style), single project in the solution, Newtonsoft.Json as the only NuGet dependency. `README.md` documents the in-game setup (macros, Auto Loot, windowed mode) and usage.

## Build & run

There are no tests and no linter. Building the solution is the whole verification gate.

```
nuget restore DefinetlyNotAFishingBot.sln   # only needed once after a fresh clone (packages/ is not tracked)
msbuild DefinetlyNotAFishingBot.sln         # Debug build → DefinetlyNotAFishingBot\bin\Debug\DefinetlyNotAFishingBot.exe
```

The app is a console executable that opens a GUI: the console prints a timestamped log of everything the bot does and accepts `start | stop | status | quit` on stdin, so the app can be driven and observed without the GUI — e.g. for a live test, pipe timed commands into it:

```
( echo start; sleep 120; echo stop; sleep 4; echo quit ) | ./DefinetlyNotAFishingBot/bin/Debug/DefinetlyNotAFishingBot.exe
```

It captures the live screen, sends input to the game, and reads/writes a config file in the user profile. A live test run presses keys and clicks in the developer's game session — only do that when the developer asks for it. Note that a running instance locks `bin\Debug\*.exe`, so it must be closed before a rebuild.

## Architecture

Two cooperating top-level windows, wired together in `frmMain.OnLoad`:

- **`frmMain`** — the settings/control window. It creates `frmCapture` (deliberately WITHOUT setting `Owner` — an owned window would minimize together with its owner, but the overlay must stay visible while frmMain is minimized during botting), constructs `ScreenManager`, creates a `FishingBot` per start (`StartBot`/`StopBot`, shared by the Start/Stop buttons, the console commands, and the panic key), and runs `GeneralTicker`, a WinForms timer that polls `ScreenManager.GetScreenCapture()` and shows the live capture in the `SelectColorPic` picture box, overlaying a lime rectangle at `FishingBot.BobberMarker` while the bot runs. Clicking that picture box maps the click back through the Zoom scaling to the exact capture pixel and stores it as `Config.BobberColor`. On start the window minimizes itself BEFORE the bot thread launches (minimizing the active window makes Windows activate some other window, and that focus churn must be over before the bot focuses WoW — otherwise the first key press lands in the wrong window), and only when its bounds overlap the capture overlay's; the status label text is colored by `BotState` (green running / yellow paused / red stopped) — the state coloring of a whole window belongs solely to the capture overlay's phase frame. A `CaughtCountLab` shows the loot count of the current run (reset on start, fed by `FishingBot.CaughtChanged`). While the bot runs, a global Escape hotkey (`RegisterHotKey`, handled in `WndProc`) stops it no matter which window has the focus and restores the window from minimized; it is unregistered on stop so Escape behaves normally in the game otherwise. A permanent hint label (`EscHintLab`) documents the Escape key in the UI. Start/Stop button handlers are wired in the constructor (not in the Designer file).
- **`ConsoleInterface`** — background thread reading stdin commands and marshaling them onto the UI thread; `frmMain` echoes every bot status line to stdout.
- **`frmCapture`** — a TopMost tool window titled "Where is the Fish?!". Its interior panel is Magenta and Magenta is the form's `TransparencyKey`, so the interior is a see-through, click-through hole: the developer drags/resizes this window over the water to define the capture region, and the bot's loot clicks pass through it into the game. Its frame color signals the bot phase (`ShowPhase`, driven by `FishingBot.PhaseChanged`): red = off, blue = fishing, green = looting. Position and size persist as `Config.CaptureBounds` — saved on `OnResizeEnd` and on close, restored in `OnHandleCreated` (never before the handle exists: pre-handle sizing goes through assumed frame metrics and drifts the window a few pixels per session) when the saved spot is still on a screen. A `doneLoading` flag ensures the programmatic startup placement never writes the config; only the developer's own moves/resizes do.
- **`ScreenManager`** — computes the screen rectangle *inside* `frmCapture`'s frame (`GetCaptureRectangle`, also used to map bobber coordinates back to screen coordinates) using the hardcoded `TOP_MARGIN`/`LEFT_MARGIN`/... constants and grabs it with `Graphics.CopyFromScreen`. The `GetScreenCapture(Rectangle)` overload captures only a sub-region (capture coordinates): the bite watcher depends on it — full-overlay captures per sample would throttle the ~30 scans/second loop to a few fps and miss the short dip. These margin constants must stay in sync with `frmCapture`'s border style and padding — changing one without the other shifts the captured region.

The bot pipeline, all `internal` classes:

- **`FishingBot`** — the bot loop on a background thread (`Start`/`Stop`, `volatile` stop flag, sleeps in 50 ms slices so stops are fast). Start sequence order is load-bearing: hide the game UI (Alt+Y, gated by `Config.HideGameUi`, restored on stop — only ever sent while WoW is focused) → outfit (equips the pole) → lure (applied onto the pole, 5 s channel) → buff, each followed by a wait for the server round trip. Loop: refish wait → park the mouse outside the capture area (a bobber under the cursor glows from mouse-over and breaks the color match) → cast → find bobber → watch for bite → on bite move the mouse onto the bobber immediately, wait the loot time, right-click, wait for the server → re-apply lure/buff on ~9-minute timers (real buffs are unreadable without memory access). It raises `StatusChanged`/`StateChanged`/`PhaseChanged`/`CaughtChanged`/`Stopped` events *from the bot thread*; `frmMain` marshals them with `BeginInvoke`. Before every key press and click it verifies WoW has the focus (`WowWindow.EnsureForeground` / `WaitForWowFocus`), re-checked immediately before the actual `SendInput` call, and the loop pauses whenever the WoW window loses focus (that is the takeover mechanism), resuming when WoW is focused again. All gameplay timing lives here as documented constants (bite window, lure interval, settle times).
- **`BobberDetector`** — LockBits pixel scan of a capture: a pixel matches when it is within `Config.ColorTollerance` per RGB channel of `Config.BobberColor` **and** keeps at least half of that color's red dominance (r−g / r−b); returns count + centroid (`BobberSample`). The dominance requirement is load-bearing — without it, murky water matches by the millions and the detection is meaningless. Bite heuristic in `FishingBot.WatchForBite`: match count collapses below half the baseline (splash swallows the bobber) or the centroid dips down by a few pixels, confirmed by two consecutive samples; a baseline covering more than half the watch region aborts with a "color matches too much water" status instead of clicking wildly.
- **`WowWindow`** — finds the 3.3.5a client window (class `GxWindowClassD3d`, fallbacks `GxWindowClass` / title "World of Warcraft") and tracks foreground state. `EnsureForeground` taps Alt before `SetForegroundWindow` so Windows honors the call even from a background process, and verifies with retries.
- **`InputSender` / `Win32`** — `SendInput` wrappers (keys as scan codes — the client reacts more reliably to them than to virtual keys — plus cursor move and right click). Works without elevation only while WoW itself is not elevated (UIPI); keep it that way — no feature may require running as administrator.

**`Config`** is the persistence layer: a JSON file at `%USERPROFILE%\.definetly_not_a_fishing_bot_config.json` (de)serialized with Newtonsoft.Json. It uses a private singleton (`MySelf`) holding instance fields, exposed through *static* properties; the static constructor calls `Reload()`, so first touch loads (or creates) the file. Every settings-UI change handler writes the value and calls `Config.Save()` immediately — there is no separate "apply" step. New settings follow the existing pattern: private `[JsonProperty]` instance field with a default value + static property wrapper.

UI conventions in `frmMain` worth knowing before editing:

- Each "set key" `Button` and its `ComboBox` are cross-linked via their `Tag` properties (button.Tag = comboBox and vice versa) so all four key bindings share one set of event handlers; the key-capture flow goes through `selectedKeySetTarget`.
- Every event handler is guarded by the `doneLoading` flag so programmatic control initialization in `OnLoad` doesn't trigger spurious `Config.Save()` calls. Keep that guard in any new handler.

## Conventions

- 2-space indentation, opening brace on the same line — match this, not the default Visual Studio style.
- Designer files (`*.Designer.cs`, `*.resx`) are generated by the WinForms designer; edit control layout there only when necessary and keep changes minimal.
