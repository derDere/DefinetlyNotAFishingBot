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
    }

    protected override void OnLoad(EventArgs e) {
      base.OnLoad(e);

      CreateAndShowCaptureWindow();

      FillKeyComboBoxes();

      SyncControlsFromConfig();

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
      UpdateSliderDisplays();
    }

    public void UpdateSliderDisplays() {
      RefishTimeDisplayLab.Text = "Refish Time: " + RefishTimeSlider.Value.ToString() + " sec";
      LootTimeDisplayLab.Text = "Loot Time: " + LootTimeSlider.Value.ToString() + " sec";
      ColorTollDisplayLab.Text = "Color tollerance: " + ColorTolleranceSlider.Value.ToString();
    }

    private void CreateAndShowCaptureWindow() {
      myCaptureWindow = new frmCapture();
      myCaptureWindow.Owner = this;
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
      Bitmap screenCap = screenManager.GetScreenCapture();
      SelectColorPic.Image = screenCap;
    }

    private void SelectColorPic_MouseClick(object sender, MouseEventArgs e) {
      Bitmap b = new Bitmap(SelectColorPic.Width, SelectColorPic.Height);
      SelectColorPic.DrawToBitmap(b, new Rectangle(0, 0, b.Width, b.Height));
      Color c = b.GetPixel(e.X, e.Y);
      Color nc = Color.FromArgb(255, 255 - c.R, 255 - c.G, 255 - c.B);
      ColorDisplayLab.BackColor = c;
      ColorDisplayLab.ForeColor = nc;
      screenManager.BobberColor = c;
    }
  }
}
