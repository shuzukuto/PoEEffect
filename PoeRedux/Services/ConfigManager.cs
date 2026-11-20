using PoeRedux.Models;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PoeRedux.Services;

public class ColorModSetting
{
    public bool Enabled { get; set; } = true;
    public string Color { get; set; } = string.Empty;
}

public class AppConfig
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PoeGame SelectedGame { get; set; } = PoeGame.PoE1;

    public string Poe1GgpkPath { get; set; } = string.Empty;
    public string Poe2GgpkPath { get; set; } = string.Empty;

    public double CameraZoom { get; set; } = 2.4;

    public bool HasPoe1Settings { get; set; }
    public List<string> Poe1SelectedPatches { get; set; } = [];
    public Dictionary<string, ColorModSetting> Poe1ColorMods { get; set; } = [];

    public bool HasPoe2Settings { get; set; }
    public List<string> Poe2SelectedPatches { get; set; } = [];
    public Dictionary<string, ColorModSetting> Poe2ColorMods { get; set; } = [];
}

/// <summary>
/// Manages loading and saving user configuration across application runs.
/// Configuration is saved in %LOCALAPPDATA%\PoeRedux\config.json.
/// </summary>
public static class ConfigManager
{
    public static string ConfigFilePath { get; set; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "PoeRedux", "config.json");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static AppConfig Load()
    {
        try
        {
            if (File.Exists(ConfigFilePath))
            {
                var json = File.ReadAllText(ConfigFilePath);
                var config = JsonSerializer.Deserialize<AppConfig>(json, JsonOptions);
                if (config != null)
                    return config;
            }
        }
        catch
        {
            // Ignore read errors and fall back to defaults
        }

        return new AppConfig();
    }

    public static void Save(AppConfig config)
    {
        try
        {
            var directory = Path.GetDirectoryName(ConfigFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(config, JsonOptions);
            File.WriteAllText(ConfigFilePath, json);
        }
        catch
        {
            // Best effort; ignore write failures
        }
    }
}
