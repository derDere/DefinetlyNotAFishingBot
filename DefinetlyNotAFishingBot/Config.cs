using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IO = System.IO;

namespace DefinetlyNotAFishingBot {
  internal class Config {

    #region Initialisierung
    private const string FILE_NAME = ".definetly_not_a_fishing_bot_config.json";

    private static Config MySelf = null;

    private static string FileName() {
      string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

      path = IO.Path.Combine(path, FILE_NAME);

      return path;
    }

    internal static void Reload() {
      string fileName = FileName();

      if (!IO.File.Exists(fileName)) {
        Save();
      }

      string jj = Encoding.UTF8.GetString(IO.File.ReadAllBytes(fileName));

      MySelf = Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(jj);
    }

    internal static void Save() {
      if (MySelf == null) {
        MySelf = new Config();
      }

      string jj = Newtonsoft.Json.JsonConvert.SerializeObject(MySelf, Newtonsoft.Json.Formatting.Indented);

      string fileName = FileName();

      IO.File.WriteAllBytes(fileName, Encoding.UTF8.GetBytes(jj));
    }

    internal static void ResetToDefaults() {
      MySelf = new Config();
      Save();
    }

    static Config() {
      Reload();
    }
    #endregion


    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Newtonsoft.Json.JsonProperty("RefishTime")]
    private int _RefishTime = 2;
    /// <summary>
    /// Time in seconds the bot will wait after looting before refishing.
    /// </summary>
    public static int RefishTime {
      get {
        return MySelf._RefishTime;
      }
      set {
        MySelf._RefishTime = value;
      }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Newtonsoft.Json.JsonProperty("LootTime")]
    private int _LootTime = 2;
    /// <summary>
    /// Time in seconds the bot will wait after a catch before looting.
    /// </summary>
    public static int LootTime {
      get {
        return MySelf._LootTime;
      }
      set {
        MySelf._LootTime = value;
      }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Newtonsoft.Json.JsonProperty("FishingKey")]
    private Keys _FishingKey = Keys.D2;
    /// <summary>
    /// Key used to fish
    /// </summary>
    public static Keys FishingKey {
      get {
        return MySelf._FishingKey;
      }
      set {
        MySelf._FishingKey = value;
      }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Newtonsoft.Json.JsonProperty("LureKey")]
    private Keys _LureKey = Keys.D0;
    /// <summary>
    /// The Key used to apply a Lure.
    /// </summary>
    public static Keys LureKey {
      get {
        return MySelf._LureKey;
      }
      set {
        MySelf._LureKey = value;
      }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Newtonsoft.Json.JsonProperty("BuffKey")]
    private Keys _BuffKey = Keys.Oem4;
    /// <summary>
    /// The key used the apply a fishing buff
    /// </summary>
    public static Keys BuffKey {
      get {
        return MySelf._BuffKey;
      }
      set {
        MySelf._BuffKey = value;
      }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Newtonsoft.Json.JsonProperty("OutfitKey")]
    private Keys _OutfitKey = Keys.Oem6;
    /// <summary>
    /// The key used to change into the right outfit.
    /// </summary>
    public static Keys OutfitKey {
      get {
        return MySelf._OutfitKey;
      }
      set {
        MySelf._OutfitKey = value;
      }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Newtonsoft.Json.JsonProperty("ColorTollerance")]
    private int _ColorTollerance = 30;
    /// <summary>
    /// Per-channel tolerance (0-255) when matching pixels against the bobber color.
    /// </summary>
    public static int ColorTollerance {
      get {
        return MySelf._ColorTollerance;
      }
      set {
        MySelf._ColorTollerance = value;
      }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Newtonsoft.Json.JsonProperty("BobberColorArgb")]
    private int _BobberColorArgb = unchecked((int)0xFF5C3227);
    /// <summary>
    /// The bobber color the developer picked from the capture preview, stored as
    /// an ARGB value. Zero means no color has been picked yet; defaults to the
    /// bobber feather color (#5C3227) picked on the dev machine.
    /// </summary>
    public static Color BobberColor {
      get {
        return Color.FromArgb(MySelf._BobberColorArgb);
      }
      set {
        MySelf._BobberColorArgb = value.ToArgb();
      }
    }

    /// <summary>True once the developer has picked a bobber color.</summary>
    public static bool HasBobberColor {
      get {
        return MySelf._BobberColorArgb != 0;
      }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Newtonsoft.Json.JsonProperty("HideGameUi")]
    private bool _HideGameUi = true;
    /// <summary>
    /// Whether the bot hides the WoW interface with Alt+Y while it runs, so no
    /// UI element can cover the bobber or disturb the pixel detection.
    /// </summary>
    public static bool HideGameUi {
      get {
        return MySelf._HideGameUi;
      }
      set {
        MySelf._HideGameUi = value;
      }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Newtonsoft.Json.JsonProperty("CaptureX")]
    private int _CaptureX = 1086;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Newtonsoft.Json.JsonProperty("CaptureY")]
    private int _CaptureY = 159;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Newtonsoft.Json.JsonProperty("CaptureWidth")]
    private int _CaptureWidth = 3140;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Newtonsoft.Json.JsonProperty("CaptureHeight")]
    private int _CaptureHeight = 820;
    /// <summary>
    /// Position and size of the capture overlay window. Updated whenever the
    /// developer moves or resizes the overlay, so it comes back up in the same
    /// spot next time.
    /// </summary>
    public static Rectangle CaptureBounds {
      get {
        return new Rectangle(MySelf._CaptureX, MySelf._CaptureY, MySelf._CaptureWidth, MySelf._CaptureHeight);
      }
      set {
        MySelf._CaptureX = value.X;
        MySelf._CaptureY = value.Y;
        MySelf._CaptureWidth = value.Width;
        MySelf._CaptureHeight = value.Height;
      }
    }
  }
}
