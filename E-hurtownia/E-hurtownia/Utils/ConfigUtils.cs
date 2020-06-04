using System;
using Utils;

namespace E_hurtownia.Utils
{
    public class ConfigUtils
    {
        public static string CONFIG_INI_PATH = "config.ini";
        public static string VERSION_TXT_PATH = "version.txt";

        public static string GetDbConnectionString()
        {
            return GetValueFromFile(ConfigParams.CONNECTION_STRING, CONFIG_INI_PATH);
        }

        public static string GetVersion()
        {
            return GetValueFromFile(ConfigParams.APP_VERSION, VERSION_TXT_PATH);
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