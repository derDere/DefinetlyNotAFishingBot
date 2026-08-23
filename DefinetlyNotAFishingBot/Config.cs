using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    private int _ColorTollerance = 5;
    /// <summary>
    /// Defines how much the bobber collor can varry.
    /// </summary>
    public static int ColorTollerance {
      get {
        return MySelf._ColorTollerance;
      }
      set {
        MySelf._ColorTollerance = value;
      }
    }
  }
}
