using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DefinetlyNotAFishingBot {
  public partial class frmCapture : Form {

    /// <summary>
    /// True once the window is fully up; programmatic placement during startup
    /// must never write the config, only the developer's own moves/resizes do.
    /// </summary>
    private bool doneLoading = false;

    public frmCapture() {
      InitializeComponent();
      // The saved position is applied in OnHandleCreated: setting the size
      // before the window handle exists makes WinForms convert it through
      // assumed frame metrics, which drifts the window by a few pixels on
      // every save/load cycle.
      StartPosition = FormStartPosition.Manual;
    }

    protected override void OnHandleCreated(EventArgs e) {
      base.OnHandleCreated(e);
      if (doneLoading)
        return; // A recreated handle must not reset the developer's position.

      // Come back up where the developer last left the window; the position is
      // persisted in the config. Skip it when that spot is no longer on any
      // screen (e.g. after a monitor change), keeping the designer defaults.
      Rectangle saved = Config.CaptureBounds;
      if (SystemInformation.VirtualScreen.IntersectsWith(saved))
        Bounds = saved;
    }

    protected override void OnShown(EventArgs e) {
      base.OnShown(e);
      doneLoading = true;
    }

    /// <summary>
    /// Colors the window frame by bot phase, so the developer sees at a glance
    /// what the bot is doing: red = bot off, blue = fishing, green = looting.
    /// </summary>
    internal void ShowPhase(BotPhase phase) {
      switch (phase) {
        case BotPhase.Fishing:
          BackColor = Color.DodgerBlue;
          break;
        case BotPhase.Looting:
          BackColor = Color.LimeGreen;
          break;
        default:
          BackColor = Color.Red;
          break;
      }
    }

    /// <summary>Persists position and size once the developer finished moving or resizing the window.</summary>
    protected override void OnResizeEnd(EventArgs e) {
      base.OnResizeEnd(e);
      SaveBounds();
    }

    protected override void OnFormClosing(FormClosingEventArgs e) {
      // Also save on close so the final position is never lost.
      SaveBounds();
      base.OnFormClosing(e);
    }

    private void SaveBounds() {
      if (!doneLoading)
        return;
      Config.CaptureBounds = Bounds;
      Config.Save();
    }
  }
}
