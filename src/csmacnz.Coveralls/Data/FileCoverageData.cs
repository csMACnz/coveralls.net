using System;
using Newtonsoft.Json;

namespace csmacnz.Coveralls.Data
{
    public class FileCoverageData
    {
        public FileCoverageData(string fullPath, int?[] coverage)
            : this(fullPath, coverage, Array.Empty<string>())
        {
        }

        public FileCoverageData(string fullPath, int?[] coverage, string[] source)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                throw new ArgumentException(
                    message: $"Parameter '{nameof(fullPath)}' must have a value (and not be empty string).",
                    paramName: nameof(fullPath));
            }

            FullPath = fullPath;
            Coverage = coverage ?? throw new ArgumentNullException(nameof(coverage));
            Source = source;
        }

        [JsonIgnore]
        public string FullPath { get; }

        [JsonProperty("full_path")]
        private string UrlEncodedFullPath
        {
            get { return System.Web.HttpUtility.UrlEncode(FullPath); }
        }

        public int?[] Coverage { get; }

        public string[] Source { get; }
    }
}
