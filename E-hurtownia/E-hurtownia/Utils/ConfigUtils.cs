using System;
using Utils;

namespace E_hurtownia.Utils
{
    public class ConfigUtil
    {
        public static string Version { get; private set; } = "";
        public static string ConfigIniPath { get; private set; }
        
        public static void SetVersion(string version)
        {
            Version = version;
        }

        public static string GetDbConnectionString()
        {
            return GetValueFromFile(ConfigParamKeys.CONNECTION_STRING, ConfigIniPath);
        }

        internal static void SetConfigIniPath(string path)
        {
            ConfigIniPath = path;
        }

        public static string GetVersion()
        {
            return Version;
        }

        public static string GetValueFromFile(string key, string filePath)
        {
            Ini config = new Ini(filePath);
            if (config.ContainsKey(key))
                return config[key];
            else
                throw new Exception("Key: " + key + "not found in file:" + filePath);
        }
    }
}