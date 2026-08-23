using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DefinetlyNotAFishingBot {
  public partial class frmMain : Form {

    ScreenManager screenManager;
    frmCapture myCaptureWindow = null;
    FishingBot fishingBot = null;
    ComboBox selectedKeySetTarget = null;
    bool doneLoading = false;

    public frmMain() {
      InitializeComponent();

      // Used to handle all 4 in the same event handler
      FishingKeySetBtn.Tag = FishingKeyCB;
      LureKeySetBtn.Tag = LureKeyCB;
      BuffKeySetBtn.Tag = BuffKeyCB;
      OutfitKeySetBtn.Tag = OutfitKeyCB;

      FishingKeyCB.Tag = FishingKeySetBtn;
      LureKeyCB.Tag = LureKeySetBtn;
      BuffKeyCB.Tag = BuffKeySetBtn;
      OutfitKeyCB.Tag = OutfitKeySetBtn;

      StartBtn.Click += StartBtn_Click;
      StopBtn.Click += StopBtn_Click;
      HideUiChk.CheckedChanged += HideUiChk_CheckedChanged;
    }

    protected override void OnLoad(EventArgs e) {
      base.OnLoad(e);

      CreateAndShowCaptureWindow();

      FillKeyComboBoxes();

      SyncControlsFromConfig();

      StopBtn.Enabled = false;

      doneLoading = true;

      GeneralTicker.Start();
    }

    private static void SetCbKeySelected(ComboBox cb, Keys key) {
      foreach (ComboBoxItem<Keys> item in cb.Items) {
        if (item.Value == key) {
          cb.SelectedItem = item;
          break;
        }
      }
    }

    private void SyncControlsFromConfig() {
      SetCbKeySelected(FishingKeyCB, Config.FishingKey);
      SetCbKeySelected(LureKeyCB, Config.LureKey);
      SetCbKeySelected(BuffKeyCB, Config.BuffKey);
      SetCbKeySelected(OutfitKeyCB, Config.OutfitKey);
      RefishTimeSlider.Value = Config.RefishTime;
      LootTimeSlider.Value = Config.LootTime;
      ColorTolleranceSlider.Value = Config.ColorTollerance;
      HideUiChk.Checked = Config.HideGameUi;
      UpdateSliderDisplays();
      UpdateColorDisplay();
    }

    public void UpdateSliderDisplays() {
      RefishTimeDisplayLab.Text = "Refish Time: " + RefishTimeSlider.Value.ToString() + " sec";
      LootTimeDisplayLab.Text = "Loot Time: " + LootTimeSlider.Value.ToString() + " sec";
      ColorTollDisplayLab.Text = "Color tollerance: " + ColorTolleranceSlider.Value.ToString();
    }

    /// <summary>Shows the picked bobber color on the swatch label (or the neutral default when none is picked yet).</summary>
    private void UpdateColorDisplay() {
      if (Config.HasBobberColor) {
        Color c = Config.BobberColor;
        ColorDisplayLab.BackColor = c;
        ColorDisplayLab.ForeColor = Color.FromArgb(255, 255 - c.R, 255 - c.G, 255 - c.B);
      } else {
        ColorDisplayLab.BackColor = Color.White;
        ColorDisplayLab.ForeColor = Color.Black;
      }
    }

    private void CreateAndShowCaptureWindow() {
      // Deliberately NOT owned by this form: an owned window would minimize
      // together with its owner, but the capture overlay must stay visible
      // while this window is minimized during botting.
      myCaptureWindow = new frmCapture();
      myCaptureWindow.Show();

      screenManager = new ScreenManager(myCaptureWindow);
    }

    private void FillKeyComboBoxes() {
      foreach (ComboBox cb in new ComboBox[] { FishingKeyCB, LureKeyCB, BuffKeyCB, OutfitKeyCB }) {
        foreach (Keys key in Enum.GetValues(typeof(Keys))) {
          ComboBoxItem<Keys> cbItem = new ComboBoxItem<Keys>(key, Enum.GetName(typeof(Keys), key));
          cb.Items.Add(cbItem);
        }
      }
    }

    protected override void OnFormClosing(FormClosingEventArgs e) {
      // Stop the bot before the window handle goes away so no status update
      // arrives on a disposed form.
      if (fishingBot != null)
        fishingBot.Stop();
      Win32.UnregisterHotKey(Handle, HOTKEY_ID_STOP);
      base.OnFormClosing(e);
    }

    protected override void OnClosed(EventArgs e) {
      base.OnClosed(e);
      myCaptureWindow.Close();
    }

    private bool HandleKeySetSelection(Keys key) {
      if (!doneLoading)
        return false;
      if (selectedKeySetTarget != null) {
        if (selectedKeySetTarget.Tag is Button btn) {
          btn.BackColor = Color.Gainsboro;
          selectedKeySetTarget.ForeColor = Color.Black;
          SetCbKeySelected(selectedKeySetTarget, key);
          selectedKeySetTarget = null;
          return true;
        }
      }
      return false;
    }

    private void SetBtnKeyDown(object sender, KeyEventArgs e) {
      if (!doneLoading)
        return;
      if (HandleKeySetSelection(e.KeyCode)) {
        e.Handled = true;
      }
    }

    protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e) {
      if (!doneLoading)
        return;
      if (HandleKeySetSelection(e.KeyCode)) {
        return;
      }
      base.OnPreviewKeyDown(e);
    }

    protected override void OnKeyDown(KeyEventArgs e) {
      if (!doneLoading)
        return;
      if (HandleKeySetSelection(e.KeyCode)) {
        e.Handled = true;
      }
      base.OnKeyDown(e);
    }

    private void SetKeyClicked(object sender, EventArgs e) {
      if (!doneLoading)
        return;
      if (sender is Button btn) {
        if (btn.Tag is ComboBox cb) {
          selectedKeySetTarget = cb;
          btn.BackColor = Color.CornflowerBlue;
          cb.ForeColor = Color.CornflowerBlue;
        }
      }
    }

    private void FishingKeyCB_SelectedIndexChanged(object sender, EventArgs e) {
      if (!doneLoading)
        return;
      if (FishingKeyCB.SelectedItem is ComboBoxItem<Keys> itm) {
        Config.FishingKey = itm.Value;
        Config.Save();
      }
    }

    private void LureKeyCB_SelectedIndexChanged(object sender, EventArgs e) {
      if (!doneLoading)
        return;
      if (LureKeyCB.SelectedItem is ComboBoxItem<Keys> itm) {
        Config.LureKey = itm.Value;
        Config.Save();
      }
    }

    private void BuffKeyCB_SelectedIndexChanged(object sender, EventArgs e) {
      if (!doneLoading)
        return;
      if (BuffKeyCB.SelectedItem is ComboBoxItem<Keys> itm) {
        Config.BuffKey = itm.Value;
        Config.Save();
      }
    }

    private void OutfitKeyCB_SelectedIndexChanged(object sender, EventArgs e) {
      if (!doneLoading)
        return;
      if (OutfitKeyCB.SelectedItem is ComboBoxItem<Keys> itm) {
        Config.OutfitKey = itm.Value;
        Config.Save();
      }
    }

    private void RefishTimeSlider_Scroll(object sender, EventArgs e) {
      if (!doneLoading)
        return;
      Config.RefishTime = RefishTimeSlider.Value;
      Config.Save();
      UpdateSliderDisplays();
    }

    private void LootTimeSlider_Scroll(object sender, EventArgs e) {
      if (!doneLoading)
        return;
      Config.LootTime = LootTimeSlider.Value;
      Config.Save();
      UpdateSliderDisplays();
    }

    private void HideUiChk_CheckedChanged(object sender, EventArgs e) {
      if (!doneLoading)
        return;
      Config.HideGameUi = HideUiChk.Checked;
      Config.Save();
    }

    private void ColorTolleranceSlider_Scroll(object sender, EventArgs e) {
      if (!doneLoading)
        return;
      Config.ColorTollerance = ColorTolleranceSlider.Value;
      Config.Save();
      UpdateSliderDisplays();
    }

    private void ResetFormBtn_Click(object sender, EventArgs e) {
      if (MessageBox.Show("Do you really want to reset all your configurations?!", "ARE YOU SURE?!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK) {
        Config.ResetToDefaults();
        SyncControlsFromConfig();
      }
    }

    private void GeneralTicker_Tick(object sender, EventArgs e) {
      Bitmap shot = screenManager.GetScreenCapture();

      // While the bot runs, mark where it believes the bobber is, so the
      // developer can verify the detection at a glance.
      if (fishingBot != null && fishingBot.IsRunning) {
        Rectangle marker = fishingBot.BobberMarker;
        if (marker != Rectangle.Empty) {
          using (Graphics g = Graphics.FromImage(shot))
          using (Pen pen = new Pen(Color.Lime, 3f)) {
            g.DrawRectangle(pen, marker);
          }
        }
      }

      Image old = SelectColorPic.Image;
      SelectColorPic.Image = shot;
      if (old != null)
        old.Dispose();
    }

    private void SelectColorPic_MouseClick(object sender, MouseEventArgs e) {
      if (!doneLoading)
        return;
      if (!(SelectColorPic.Image is Bitmap preview))
        return;

      // The picture box is in Zoom mode: map the click position back to the
      // capture bitmap so the exact (unscaled, unblended) pixel color is picked.
      float scale = Math.Min(
        (float)SelectColorPic.ClientSize.Width / preview.Width,
        (float)SelectColorPic.ClientSize.Height / preview.Height
      );
      float offsetX = (SelectColorPic.ClientSize.Width - preview.Width * scale) / 2f;
      float offsetY = (SelectColorPic.ClientSize.Height - preview.Height * scale) / 2f;
      int imgX = (int)((e.X - offsetX) / scale);
      int imgY = (int)((e.Y - offsetY) / scale);
      if (imgX < 0 || imgY < 0 || imgX >= preview.Width || imgY >= preview.Height)
        return; // Click landed on the letterbox area outside the image.

      Config.BobberColor = preview.GetPixel(imgX, imgY);
      Config.Save();
      UpdateColorDisplay();
    }

    private void StartBtn_Click(object sender, EventArgs e) {
      StartBot();
    }

    private void StopBtn_Click(object sender, EventArgs e) {
      StopBot();
    }

    /// <summary>The current bot status line, for the console "status" command.</summary>
    internal string CurrentStatusText {
      get { return StatusLab.Text; }
    }

    /// <summary>
    /// Starts the bot (button or console command): minimizes this window (the
    /// capture overlay stays visible), registers the global Escape panic key,
    /// and launches the bot loop.
    /// </summary>
    internal void StartBot() {
      if (!doneLoading)
        return;
      if (fishingBot != null && fishingBot.IsRunning)
        return;
      if (!Config.HasBobberColor) {
        StatusLab.ForeColor = Color.DarkRed;
        StatusLab.Text = "Pick the bobber color first (click it in the preview below)!";
        return;
      }

      fishingBot = new FishingBot(screenManager);
      fishingBot.StatusChanged += OnBotStatusChanged;
      fishingBot.StateChanged += OnBotStateChanged;
      fishingBot.PhaseChanged += OnBotPhaseChanged;
      fishingBot.CaughtChanged += OnBotCaughtChanged;
      fishingBot.Stopped += OnBotStopped;

      StartBtn.Enabled = false;
      StopBtn.Enabled = true;
      ApplyStateColors(BotState.Running);
      StatusLab.Text = "Starting…";
      CaughtCountLab.Text = "Loots: 0";

      // Global panic key: Escape stops the bot no matter which window has the
      // focus (including the game). Only registered while the bot runs, so
      // Escape behaves normally in the game at all other times.
      Win32.RegisterHotKey(Handle, HOTKEY_ID_STOP, 0, (uint)Keys.Escape);

      // Minimize BEFORE the bot thread starts: minimizing the active window
      // makes Windows activate some other window, and that focus churn must be
      // over before the bot focuses WoW — otherwise the first key press can
      // land in the wrong window. Only minimized when this window would
      // otherwise show up inside the capture area.
      if (Bounds.IntersectsWith(myCaptureWindow.Bounds))
        WindowState = FormWindowState.Minimized;

      fishingBot.Start();
    }

    /// <summary>Stops the bot (button, console command, or Escape panic key).</summary>
    internal void StopBot() {
      StopBtn.Enabled = false;
      if (fishingBot != null)
        fishingBot.Stop();
    }

    /// <summary>Identifier of the global Escape hotkey registered while the bot runs.</summary>
    private const int HOTKEY_ID_STOP = 1;

    protected override void WndProc(ref Message m) {
      if (m.Msg == Win32.WM_HOTKEY && (int)m.WParam == HOTKEY_ID_STOP) {
        // Bring the window back right away so the developer sees the bot stop.
        if (WindowState == FormWindowState.Minimized)
          WindowState = FormWindowState.Normal;
        StopBot();
        return;
      }
      base.WndProc(ref m);
    }

    /// <summary>Colors the status text by bot state; the capture overlay's frame shows the activity phase.</summary>
    private void ApplyStateColors(BotState state) {
      switch (state) {
        case BotState.Running:
          StatusLab.ForeColor = Color.DarkGreen;
          break;
        case BotState.Paused:
          StatusLab.ForeColor = Color.DarkGoldenrod;
          break;
        default:
          StatusLab.ForeColor = Color.DarkRed;
          break;
      }
    }

    /// <summary>Marshals bot status messages (raised on the bot thread) onto the UI thread.</summary>
    private void OnBotStatusChanged(string text) {
      if (IsDisposed || !IsHandleCreated)
        return;
      if (InvokeRequired) {
        try {
          BeginInvoke(new Action<string>(OnBotStatusChanged), text);
        } catch (ObjectDisposedException) {
          // The form went away while the bot was shutting down — nothing to show anymore.
        }
        return;
      }
      StatusLab.Text = text;
      Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + "  " + text);
    }

    /// <summary>Marshals loot-count updates (raised on the bot thread) onto the UI thread.</summary>
    private void OnBotCaughtChanged(int count) {
      if (IsDisposed || !IsHandleCreated)
        return;
      if (InvokeRequired) {
        try {
          BeginInvoke(new Action<int>(OnBotCaughtChanged), count);
        } catch (ObjectDisposedException) {
          // The form went away while the bot was shutting down — nothing to show anymore.
        }
        return;
      }
      CaughtCountLab.Text = "Loots: " + count;
    }

    /// <summary>Marshals bot phase changes (raised on the bot thread) onto the UI thread and colors the overlay.</summary>
    private void OnBotPhaseChanged(BotPhase phase) {
      if (IsDisposed || !IsHandleCreated)
        return;
      if (InvokeRequired) {
        try {
          BeginInvoke(new Action<BotPhase>(OnBotPhaseChanged), phase);
        } catch (ObjectDisposedException) {
          // The form went away while the bot was shutting down — nothing to show anymore.
        }
        return;
      }
      if (myCaptureWindow != null && !myCaptureWindow.IsDisposed)
        myCaptureWindow.ShowPhase(phase);
    }

    /// <summary>Marshals bot state changes (raised on the bot thread) onto the UI thread.</summary>
    private void OnBotStateChanged(BotState state) {
      if (IsDisposed || !IsHandleCreated)
        return;
      if (InvokeRequired) {
        try {
          BeginInvoke(new Action<BotState>(OnBotStateChanged), state);
        } catch (ObjectDisposedException) {
          // The form went away while the bot was shutting down — nothing to show anymore.
        }
        return;
      }
      ApplyStateColors(state);
    }

    /// <summary>Restores the UI once the bot loop has terminated (regular stop or error).</summary>
    private void OnBotStopped() {
      if (IsDisposed || !IsHandleCreated)
        return;
      if (InvokeRequired) {
        try {
          BeginInvoke(new Action(OnBotStopped));
        } catch (ObjectDisposedException) {
          // The form went away while the bot was shutting down — nothing to restore.
        }
        return;
      }
      Win32.UnregisterHotKey(Handle, HOTKEY_ID_STOP);
      ApplyStateColors(BotState.Stopped);
      StartBtn.Enabled = true;
      StopBtn.Enabled = false;
      if (WindowState == FormWindowState.Minimized)
        WindowState = FormWindowState.Normal;
    }
  }
}
