using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DefinetlyNotAFishingBot {
  internal static class Program {
    /// <summary>
    /// Der Haupteinstiegspunkt für die Anwendung.
    /// </summary>
    [STAThread]
    static void Main() {
      try {
        Console.OutputEncoding = Encoding.UTF8;
      } catch (Exception) {
        // No usable console (e.g. detached) — output just goes nowhere.
      }

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      frmMain mainWindow = new frmMain();
      new ConsoleInterface(mainWindow).Start();
      Application.Run(mainWindow);
    }
  }
}
