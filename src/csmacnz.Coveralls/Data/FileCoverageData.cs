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
                throw new ArgumentException("fullPath");
            }

            if (coverage == null)
            {
                throw new ArgumentException("coverage");
            }

            FullPath = fullPath;
            Coverage = coverage;
            Source = source?.Any() == true ? source : null;
        }

        public string FullPath { get; }

        public int?[] Coverage { get; }

        public string[] Source { get; }
    }
}