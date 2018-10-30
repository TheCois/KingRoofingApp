using System;
using System.Collections.Generic;
using System.IO;

namespace KRF.Core
{
    public class SecretsReader
    {
        private readonly IDictionary<string, string> pairs_;

        public SecretsReader(string path)
        {
            var lines = File.ReadLines(path);
            pairs_ = new Dictionary<string, string>(100);
            foreach (var line in lines)
            {
                if (line.Trim() == "")
                    continue;
                var values = line.Split(',');
                if (values.Length != 2)
                {
                    var msg = $"File {path} contains an ill-defined line:'{line}'";
                    throw new ArgumentException(msg, nameof(path));
                }

                pairs_.Add(values[0].Trim(), values[1].Trim());
            }
        }

        public string this[string key] => pairs_.ContainsKey(key) ? pairs_[key] : null;

        public ICollection<string> Keys => pairs_.Keys;

        public bool Holds(string key)
        {
            return pairs_.ContainsKey(key);
        }
    }
}