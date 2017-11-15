using System;
using Newtonsoft.Json;

namespace csmacnz.Coveralls.Data
{
    public sealed class CoverageFile
    {
        public CoverageFile(string name, string sourceDigest, int?[] coverage)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("name");
            }

            if (sourceDigest == null)
            {
                throw new ArgumentException("sourceDigest");
            }

            if (coverage == null)
            {
                throw new ArgumentException("coverage");
            }

            Name = name;
            SourceDigest = sourceDigest;
            Coverage = coverage;
        }

        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("source_digest")]
        public string SourceDigest { get; private set; }

        [JsonProperty("coverage")]
        public int?[] Coverage { get; private set; }
    }
}