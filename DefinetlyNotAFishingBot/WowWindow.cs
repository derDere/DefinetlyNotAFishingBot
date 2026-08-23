using System;
using System.Threading;

namespace DefinetlyNotAFishingBot {
  /// <summary>
  /// Locates the World of Warcraft client window and tracks whether it currently
  /// has the input focus. The 3.3.5a client registers its window under the class
  /// name "GxWindowClassD3d" (older clients use "GxWindowClass"); the window
  /// title "World of Warcraft" is used as a last resort.
  /// </summary>
  internal class WowWindow {

    private IntPtr handle = IntPtr.Zero;

    /// <summary>Tries to locate the WoW client window. Returns true when found.</summary>
    internal bool Find() {
      handle = Win32.FindWindow("GxWindowClassD3d", null);
      if (handle == IntPtr.Zero)
        handle = Win32.FindWindow("GxWindowClass", null);
      if (handle == IntPtr.Zero)
        handle = Win32.FindWindow(null, "World of Warcraft");
      return handle != IntPtr.Zero;
    }

    /// <summary>True while the WoW window still exists and is the foreground window.</summary>
    internal bool IsForeground {
      get { return handle != IntPtr.Zero && Win32.IsWindow(handle) && Win32.GetForegroundWindow() == handle; }
    }

    /// <summary>
    /// Actively brings the WoW window to the foreground and verifies it worked,
    /// retrying a few times. Restores the window first if it is minimized, and
    /// taps Alt before each attempt so Windows honors SetForegroundWindow even
    /// when this process is not the foreground process. Returns true when WoW
    /// really has the focus afterwards.
    /// </summary>
    internal bool EnsureForeground() {
      const int ATTEMPTS = 4;
      const int SETTLE_MS = 400;
      for (int attempt = 0; attempt < ATTEMPTS; attempt++) {
        if (IsForeground)
          return true;
        if (handle == IntPtr.Zero)
          return false;
        if (Win32.IsIconic(handle))
          Win32.ShowWindow(handle, Win32.SW_RESTORE);
        InputSender.TapAlt();
        Win32.SetForegroundWindow(handle);
        Thread.Sleep(SETTLE_MS);
      }
      return IsForeground;
    }
  }
}
