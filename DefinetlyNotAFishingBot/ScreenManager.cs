using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefinetlyNotAFishingBot {
  internal class ScreenManager {

    private const int TOP_MARGIN = 34;
    private const int LEFT_MARGIN = 11;
    private const int RIGHT_MARGIN = 11;
    private const int BOTTOM_MARGIN = 11;

    public Color BobberColor { get; set; } = Color.White;

    frmCapture myCaptureWindow;

    internal ScreenManager(frmCapture myCaptureWindow) {
      this.myCaptureWindow = myCaptureWindow;
    }

    internal Bitmap GetScreenCapture() {
      Rectangle captureSize = new Rectangle(
        myCaptureWindow.Left + LEFT_MARGIN,
        myCaptureWindow.Top + TOP_MARGIN,
        myCaptureWindow.Width - LEFT_MARGIN - RIGHT_MARGIN,
        myCaptureWindow.Height - TOP_MARGIN - BOTTOM_MARGIN
      );

      Rectangle imageSize = new Rectangle(
        0, 0,
        captureSize.Width,
        captureSize.Height
      );

      Bitmap b = new Bitmap(imageSize.Width, imageSize.Height);

      Graphics g = Graphics.FromImage(b);

      g.CopyFromScreen(
        captureSize.Location,
        imageSize.Location,
        imageSize.Size
      );

      return b;
    }
  }
}
