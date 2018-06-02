using System;
using System.Collections.Generic;
using System.Linq;
using csmacnz.Coveralls.Data;

namespace csmacnz.Coveralls
{
    public class FileCoverageDataBuilder
    {
        private readonly Dictionary<int, int> _coverage = new Dictionary<int, int>();
        private readonly string _filePath;

        public FileCoverageDataBuilder(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException($"Parameter '{nameof(filePath)}' must have a value (and not be empty string).", nameof(filePath));
            }

            _filePath = filePath;
        }

        public void RecordCoverage(int lineNumber, int coverageNumber)
        {
            _coverage[lineNumber - 1] = coverageNumber;
        }

        public FileCoverageData CreateFile()
        {
            var length = _coverage.Any() ? _coverage.Max(c => c.Key) + 1 : 1;
            var coverage = Enumerable.Range(0, length)
                .Select(index => _coverage.ContainsKey(index) ? (int?)_coverage[index] : null)
                .ToArray();
            return new FileCoverageData(_filePath, coverage);
        }
    }
}
