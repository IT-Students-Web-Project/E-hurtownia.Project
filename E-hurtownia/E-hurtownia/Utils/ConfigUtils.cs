using System;
using Utils;

namespace E_hurtownia.Utils
{
    public class ConfigUtils
    {
        public static string CONFIG_INI_PATH = "config.ini";

        public static string GetDbConnectionString()
        {
            Ini config = new Ini(CONFIG_INI_PATH);
            if (config.ContainsKey("connectionString"))
                return config["connectionString"];
            else
                throw new Exception("Key: 'connectionString' not found in file:" + CONFIG_INI_PATH);
        }
    }
}