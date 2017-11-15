using System;
using System.Linq;

namespace csmacnz.Coveralls.Data
{
    public class FileCoverageData
    {
        public FileCoverageData(string fullPath, int?[] coverage)
            : this(fullPath, coverage, null)
        {
        }

        public FileCoverageData(string fullPath, int?[] coverage, string[] source)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                throw new ArgumentException($"Parameter '{nameof(fullPath)}' must have a value (and not be empty string).", nameof(fullPath));
            }

            FullPath = fullPath;
            Coverage = coverage ?? throw new ArgumentNullException(nameof(coverage));
            Source = source?.Any() == true ? source : null;
        }

        public string FullPath { get; }

        public int?[] Coverage { get; }

        public string[] Source { get; }
    }
}
