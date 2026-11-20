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
/// Configuration is saved in config.json alongside the application executable.
/// </summary>
public static class ConfigManager
{
    private static string ExeDirectory =>
        Path.GetDirectoryName(Environment.ProcessPath) ?? AppContext.BaseDirectory;

    public static string ConfigFilePath { get; set; } = Path.Combine(ExeDirectory, "config.json");

    private static readonly string FallbackConfigFilePath = Path.Combine(
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
            // Primary location: alongside the executable
            if (File.Exists(ConfigFilePath))
            {
                var json = File.ReadAllText(ConfigFilePath);
                var config = JsonSerializer.Deserialize<AppConfig>(json, JsonOptions);
                if (config != null)
                    return config;
            }

            // Fallback / migration: check AppData if it was previously saved there
            if (File.Exists(FallbackConfigFilePath))
            {
                var json = File.ReadAllText(FallbackConfigFilePath);
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
            // If saving next to executable fails (e.g. read-only folder), fall back to AppData
            try
            {
                var fallbackDir = Path.GetDirectoryName(FallbackConfigFilePath);
                if (!string.IsNullOrEmpty(fallbackDir) && !Directory.Exists(fallbackDir))
                {
                    Directory.CreateDirectory(fallbackDir);
                }

                var json = JsonSerializer.Serialize(config, JsonOptions);
                File.WriteAllText(FallbackConfigFilePath, json);
            }
            catch
            {
                // Best effort; ignore write failures
            }
        }
    }
}
