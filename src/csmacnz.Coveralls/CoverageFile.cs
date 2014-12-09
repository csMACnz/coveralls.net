using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csmacnz.Coveralls
{
    public sealed class CoverageFile
    {
        public CoverageFile(string name, string[] source, int?[] coverage)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("name");
            if (source == null) throw new ArgumentException("source");
            if (coverage == null) throw new ArgumentException("coverage");
            if(coverage.Length != source.Length) throw new ArgumentOutOfRangeException("coverage");
            
            Name = name;
            Source = string.Join("\n",source);
            Coverage = coverage;
        }

        [JsonProperty("name")]
        public string Name { get; private set; }
        
        [JsonProperty("source")]
        public string Source { get; private set; }
        
        [JsonProperty("coverage")]
        public int?[] Coverage { get; private set; }
    }
}
