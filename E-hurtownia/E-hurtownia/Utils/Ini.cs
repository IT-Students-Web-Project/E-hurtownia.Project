using System;
using System.Collections.Generic;
using System.IO;

namespace Utils
{
    public class Ini
    {
        private Dictionary<string, string> data;
        public String Path
        {
            get;
        }
        public Ini(string path)
        {
            if (!File.Exists(path))
                throw new Exception("Nie znaleziono pliku: " + path);
            
            Path = path;
            data = new Dictionary<string, string>();
            Load();
        }

        private void Load()
        {            
            foreach (var line in File.ReadAllLines(this.Path))
            {
                int splitIndex = line.IndexOf('=');
                string key = line.Substring(0, splitIndex);
                string value = line.Substring(splitIndex + 1);
                data.Add(key, value);
            }
        }

        public String this[String key]
        {
            get { return this.data[key]; }
            set { this.data[key] = value; }
        }

        public bool ContainsKey(String key)
        {
            return data.ContainsKey(key);
        }
    }
}
