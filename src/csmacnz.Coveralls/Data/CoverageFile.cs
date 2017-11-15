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
                throw new ArgumentException($"Parameter '{nameof(name)}' must have a value (and not be empty string).", nameof(name));
            }

            Name = name;
            SourceDigest = sourceDigest ?? throw new ArgumentNullException(nameof(sourceDigest));
            Coverage = coverage ?? throw new ArgumentNullException(nameof(coverage));
        }

        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("source_digest")]
        public string SourceDigest { get; private set; }

        [JsonProperty("coverage")]
        public int?[] Coverage { get; private set; }
    }
}
