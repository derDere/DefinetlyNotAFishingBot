using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace DefinetlyNotAFishingBot {

  /// <summary>
  /// Result of one pixel scan: how many pixels matched the bobber color and
  /// where their center of mass is (in image coordinates, only valid when
  /// MatchCount is greater than zero).
  /// </summary>
  internal struct BobberSample {
    internal int MatchCount;
    internal Point Centroid;
  }

  /// <summary>
  /// Finds the fishing bobber in a screen capture. A pixel matches when it is
  /// within a per-channel tolerance of the user-picked bobber color AND shows
  /// at least half of that color's red dominance (r-g / r-b distances). The
  /// dominance requirement is what separates the bobber's red feather from
  /// water and terrain, which are often brownish but never as red-dominant;
  /// without it, murky water matches by the millions. Uses LockBits so a
  /// full-region scan only takes a few milliseconds.
  /// </summary>
  internal static class BobberDetector {

    /// <summary>Scans the whole image for the bobber color.</summary>
    internal static BobberSample FindBobber(Bitmap image, Color target, int tolerance) {
      return Scan(image, new Rectangle(Point.Empty, image.Size), target, tolerance);
    }

    /// <summary>
    /// Scans the given region of the image and returns the match count and the
    /// centroid of all matching pixels. The region is clamped to the image bounds.
    /// </summary>
    internal static BobberSample Scan(Bitmap image, Rectangle region, Color target, int tolerance) {
      BobberSample result = new BobberSample();

      Rectangle bounds = Rectangle.Intersect(region, new Rectangle(Point.Empty, image.Size));
      if (bounds.Width <= 0 || bounds.Height <= 0)
        return result;

      // The pixel must keep at least half of the target's channel separation;
      // for targets without red dominance this degrades to no constraint.
      int minRedOverGreen = (target.R - target.G) / 2;
      int minRedOverBlue = (target.R - target.B) / 2;

      BitmapData data = image.LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
      try {
        byte[] pixels = new byte[data.Stride * bounds.Height];
        Marshal.Copy(data.Scan0, pixels, 0, pixels.Length);

        long sumX = 0;
        long sumY = 0;
        int count = 0;

        for (int y = 0; y < bounds.Height; y++) {
          int row = y * data.Stride;
          for (int x = 0; x < bounds.Width; x++) {
            int i = row + x * 4;
            // Pixel layout is BGRA.
            int b = pixels[i];
            int g = pixels[i + 1];
            int r = pixels[i + 2];
            if (Math.Abs(r - target.R) <= tolerance
                && Math.Abs(g - target.G) <= tolerance
                && Math.Abs(b - target.B) <= tolerance
                && (r - g) >= minRedOverGreen
                && (r - b) >= minRedOverBlue) {
              sumX += x;
              sumY += y;
              count++;
            }
          }
        }

        result.MatchCount = count;
        if (count > 0)
          result.Centroid = new Point(bounds.X + (int)(sumX / count), bounds.Y + (int)(sumY / count));
      } finally {
        image.UnlockBits(data);
      }

      return result;
    }
  }
}
