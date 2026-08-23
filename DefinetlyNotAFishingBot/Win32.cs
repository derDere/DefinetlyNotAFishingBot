using System;
using System.Runtime.InteropServices;

namespace DefinetlyNotAFishingBot {
  /// <summary>
  /// Raw Win32 API declarations used to find the WoW window and to synthesize
  /// keyboard and mouse input. Everything here works without administrator rights
  /// as long as the target process runs at the same integrity level, i.e. as long
  /// as WoW itself is not started "as administrator" (UIPI only blocks input sent
  /// to higher-integrity processes).
  /// </summary>
  internal static class Win32 {

    internal const uint INPUT_MOUSE = 0;
    internal const uint INPUT_KEYBOARD = 1;

    internal const uint KEYEVENTF_KEYUP = 0x0002;
    internal const uint KEYEVENTF_SCANCODE = 0x0008;

    internal const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
    internal const uint MOUSEEVENTF_RIGHTUP = 0x0010;

    internal const uint MAPVK_VK_TO_VSC = 0;

    internal const int SW_RESTORE = 9;

    internal const int WM_HOTKEY = 0x0312;

    [StructLayout(LayoutKind.Sequential)]
    internal struct KEYBDINPUT {
      internal ushort wVk;
      internal ushort wScan;
      internal uint dwFlags;
      internal uint time;
      internal IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MOUSEINPUT {
      internal int dx;
      internal int dy;
      internal uint mouseData;
      internal uint dwFlags;
      internal uint time;
      internal IntPtr dwExtraInfo;
    }

    /// <summary>
    /// The union part of the INPUT struct: mouse and keyboard data share the
    /// same memory, selected by the INPUT.type field.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct INPUTUNION {
      [FieldOffset(0)] internal MOUSEINPUT mi;
      [FieldOffset(0)] internal KEYBDINPUT ki;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct INPUT {
      internal uint type;
      internal INPUTUNION u;
    }

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    [DllImport("user32.dll")]
    internal static extern uint MapVirtualKey(uint uCode, uint uMapType);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool SetCursorPos(int x, int y);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    internal static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    internal static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    internal static extern bool IsWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    internal static extern bool IsIconic(IntPtr hWnd);

    [DllImport("user32.dll")]
    internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    internal static extern bool UnregisterHotKey(IntPtr hWnd, int id);
  }
}
