using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefinetlyNotAFishingBot {
  /// <summary>
  /// Captures the screen area framed by the capture overlay window. The margins
  /// cut off the overlay's own border so only the see-through interior (i.e. the
  /// game behind it) ends up in the capture. They must stay in sync with the
  /// overlay's border style and padding.
  /// </summary>
  internal class ScreenManager {

    private const int TOP_MARGIN = 34;
    private const int LEFT_MARGIN = 11;
    private const int RIGHT_MARGIN = 11;
    private const int BOTTOM_MARGIN = 11;

    frmCapture myCaptureWindow;

    internal ScreenManager(frmCapture myCaptureWindow) {
      this.myCaptureWindow = myCaptureWindow;
    }

    /// <summary>
    /// The screen rectangle inside the overlay's frame. Also used to map bobber
    /// coordinates from a capture back to absolute screen coordinates.
    /// </summary>
    internal Rectangle GetCaptureRectangle() {
      return new Rectangle(
        myCaptureWindow.Left + LEFT_MARGIN,
        myCaptureWindow.Top + TOP_MARGIN,
        Math.Max(1, myCaptureWindow.Width - LEFT_MARGIN - RIGHT_MARGIN),
        Math.Max(1, myCaptureWindow.Height - TOP_MARGIN - BOTTOM_MARGIN)
      );
    }

    /// <summary>Takes a fresh screenshot of the capture rectangle. The caller owns (and disposes) the bitmap.</summary>
    internal Bitmap GetScreenCapture() {
      Rectangle captureSize = GetCaptureRectangle();

      Bitmap b = new Bitmap(captureSize.Width, captureSize.Height);

      using (Graphics g = Graphics.FromImage(b)) {
        g.CopyFromScreen(captureSize.Location, Point.Empty, captureSize.Size);
      }

      return b;
    }
  }
}
