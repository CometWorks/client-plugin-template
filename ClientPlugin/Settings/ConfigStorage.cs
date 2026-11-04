using System;
using System.IO;
using System.Xml.Serialization;
using VRage.FileSystem;
using VRage.Utils;

namespace ClientPlugin.Settings;

public static class ConfigStorage
{
    private static readonly string ConfigFileName = string.Concat(Plugin.Name, ".cfg");
    private static string ConfigDirectory =>
        Path.Combine(MyFileSystem.UserDataPath, "Storage");
    private static string ConfigFilePath =>
        Path.Combine(ConfigDirectory, ConfigFileName);

    private static bool? isNewDeployment;

    // True if the config file did not exist yet when this was first read, which means a new
    // deployment. It keeps its value after the first save writes the file.
    public static bool IsNewDeployment => isNewDeployment ??= !File.Exists(ConfigFilePath);

    public static void Save(Config config)
    {
        Directory.CreateDirectory(ConfigDirectory);
        using var text = File.CreateText(ConfigFilePath);
        new XmlSerializer(typeof(Config)).Serialize(text, config);
    }

    public static Config Load()
    {
        _ = IsNewDeployment;

        var path = ConfigFilePath;
        if (!File.Exists(path))
        {
            return Config.Default;
        }

        var xmlSerializer = new XmlSerializer(typeof(Config));
        try
        {
            using var streamReader = File.OpenText(path);
            return (Config)xmlSerializer.Deserialize(streamReader) ?? Config.Default;
        }
        catch (Exception)
        {
            MyLog.Default.Warning(
                $"{ConfigFileName}: Failed to read config file: {ConfigFilePath}"
            );
        }

        return Config.Default;
    }
}
