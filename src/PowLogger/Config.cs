using System.IO;
using Newtonsoft.Json;

namespace PowLogger
{
  public static class Config
  {
#if DEBUG
    private const string ConfigFile = "./../../../config.json";
#else
      private const string ConfigFile = "/app/config/config.json";
#endif
    public static AppConfig AppConfig { get; private set; }

    // Constructor
    static Config()
    {
      LoadConfigFile();
    }

    // Helper methods
    private static void LoadConfigFile()
    {
      if (!File.Exists(ConfigFile))
      {
        Log.Error($"No configuration file found: {ConfigFile}");
        return;
      }

      var contents = File.ReadAllText(ConfigFile);
      Log.Debug($"Config file found: {contents}");
      AppConfig = JsonConvert.DeserializeObject<AppConfig>(contents);
    }
  }
}

