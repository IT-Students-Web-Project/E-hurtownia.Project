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
                throw new Exception("Nie znaleziono klucza connectionString w pliku :" + CONFIG_INI_PATH);
        }
    }
}