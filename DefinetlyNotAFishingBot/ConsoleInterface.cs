using System;
using System.Threading;
using System.Windows.Forms;

namespace DefinetlyNotAFishingBot {
  /// <summary>
  /// Text command interface on stdin/stdout. The application is built as a
  /// console executable, so starting it from a terminal (or with redirected
  /// pipes) allows driving the bot without touching the GUI:
  ///
  ///   start   – same as clicking the Start button
  ///   stop    – same as clicking the Stop button
  ///   status  – prints the current status line
  ///   quit    – closes the application
  ///
  /// All bot status updates are echoed to stdout as timestamped lines. When the
  /// program is started by double-click the console window can simply be
  /// minimized; when stdin is closed the reader thread just ends and the GUI
  /// keeps running normally.
  /// </summary>
  internal class ConsoleInterface {

    private readonly frmMain mainWindow;

    internal ConsoleInterface(frmMain mainWindow) {
      this.mainWindow = mainWindow;
    }

    /// <summary>Starts the stdin reader on a background thread.</summary>
    internal void Start() {
      Console.WriteLine("Console ready — commands: start | stop | status | quit");
      Thread reader = new Thread(ReadLoop) { IsBackground = true, Name = "ConsoleInterface" };
      reader.Start();
    }

    /// <summary>Reads commands line by line until stdin is closed or "quit" arrives.</summary>
    private void ReadLoop() {
      // The window handle must exist before commands can be marshaled onto the
      // UI thread; piped commands may arrive faster than the form loads.
      while (!mainWindow.IsHandleCreated && !mainWindow.IsDisposed)
        Thread.Sleep(100);

      string line;
      while ((line = Console.ReadLine()) != null) {
        string command = line.Trim().ToLowerInvariant();
        if (command.Length == 0)
          continue;

        switch (command) {
          case "start":
            OnUi(delegate { mainWindow.StartBot(); });
            break;
          case "stop":
            OnUi(delegate { mainWindow.StopBot(); });
            break;
          case "status":
            OnUi(delegate { Console.WriteLine("STATUS: " + mainWindow.CurrentStatusText); });
            break;
          case "quit":
          case "exit":
            OnUi(delegate { mainWindow.Close(); });
            return;
          default:
            Console.WriteLine("Unknown command: " + command + " (start | stop | status | quit)");
            break;
        }
      }
    }

    /// <summary>Runs an action on the UI thread, ignoring races with a closing form.</summary>
    private void OnUi(Action action) {
      try {
        if (!mainWindow.IsDisposed && mainWindow.IsHandleCreated)
          mainWindow.BeginInvoke(action);
      } catch (ObjectDisposedException) {
        // The form closed between the check and the call — nothing left to do.
      } catch (InvalidOperationException) {
        // Same race, different exception depending on timing.
      }
    }
  }
}
