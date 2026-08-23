using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace DefinetlyNotAFishingBot {
  /// <summary>
  /// Synthesizes keyboard and mouse input via SendInput. Keys are sent as hardware
  /// scan codes because the WoW client reads low-level input and reacts more
  /// reliably to scan codes than to plain virtual-key events. The target window
  /// must have the input focus for any of this to reach the game.
  /// </summary>
  internal static class InputSender {

    /// <summary>How long a key is held down, so the game reliably registers the press.</summary>
    private const int KEY_HOLD_MS = 100;
    /// <summary>How long a mouse button is held down for a click.</summary>
    private const int BUTTON_HOLD_MS = 80;

    /// <summary>Presses and releases the given key in the foreground window.</summary>
    internal static void PressKey(Keys key) {
      SendKey(key, true);
      Thread.Sleep(KEY_HOLD_MS);
      SendKey(key, false);
    }

    /// <summary>
    /// Taps the Alt key. Windows then treats this process as the source of the
    /// last input event, which releases the foreground lock so a following
    /// SetForegroundWindow call is honored even from a background process.
    /// </summary>
    internal static void TapAlt() {
      SendKey(Keys.Menu, true);
      Thread.Sleep(40);
      SendKey(Keys.Menu, false);
    }

    /// <summary>Presses Alt+key (e.g. Alt+Y to toggle the WoW UI) in the foreground window.</summary>
    internal static void PressAltCombo(Keys key) {
      SendKey(Keys.Menu, true);
      Thread.Sleep(60);
      SendKey(key, true);
      Thread.Sleep(KEY_HOLD_MS);
      SendKey(key, false);
      Thread.Sleep(60);
      SendKey(Keys.Menu, false);
    }

    /// <summary>Moves the real mouse cursor to an absolute screen position.</summary>
    internal static void MoveCursor(Point screenPoint) {
      Win32.SetCursorPos(screenPoint.X, screenPoint.Y);
    }

    /// <summary>Performs a right click at the current cursor position.</summary>
    internal static void RightClick() {
      SendMouseButton(Win32.MOUSEEVENTF_RIGHTDOWN);
      Thread.Sleep(BUTTON_HOLD_MS);
      SendMouseButton(Win32.MOUSEEVENTF_RIGHTUP);
    }

    /// <summary>Sends a single key-down or key-up event, preferring the scan-code form.</summary>
    private static void SendKey(Keys key, bool down) {
      ushort vk = (ushort)(key & Keys.KeyCode);
      ushort scan = (ushort)Win32.MapVirtualKey(vk, Win32.MAPVK_VK_TO_VSC);

      Win32.INPUT input = new Win32.INPUT { type = Win32.INPUT_KEYBOARD };
      if (scan != 0) {
        input.u.ki.wScan = scan;
        input.u.ki.dwFlags = Win32.KEYEVENTF_SCANCODE | (down ? 0 : Win32.KEYEVENTF_KEYUP);
      } else {
        // No scan code known for this key: fall back to the virtual-key form.
        input.u.ki.wVk = vk;
        input.u.ki.dwFlags = down ? 0 : Win32.KEYEVENTF_KEYUP;
      }

      Win32.SendInput(1, new Win32.INPUT[] { input }, Marshal.SizeOf(typeof(Win32.INPUT)));
    }

    /// <summary>Sends a single mouse button event at the current cursor position.</summary>
    private static void SendMouseButton(uint buttonFlag) {
      Win32.INPUT input = new Win32.INPUT { type = Win32.INPUT_MOUSE };
      input.u.mi.dwFlags = buttonFlag;
      Win32.SendInput(1, new Win32.INPUT[] { input }, Marshal.SizeOf(typeof(Win32.INPUT)));
    }
  }
}
