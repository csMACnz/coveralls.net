using System;
using System.Collections.Generic;
using System.Linq;

namespace csmacnz.Coveralls
{
    public class CoverageFileBuilder
    {
        private readonly string _filePath;
        private readonly Dictionary<int,int> _coverage = new Dictionary<int, int>();

        public CoverageFileBuilder(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("filePath");
            }
            _filePath = filePath;
        }

        public void RecordCovered(int lineNumber)
        {
            _coverage[lineNumber-1] = 1;
        }

        public void RecordUnCovered(int lineNumber)
        {
            _coverage[lineNumber-1] = 0;
        }

        public CoverageFile CreateFile()
        {
            var length = _coverage.Any() ? _coverage.Max(c => c.Key) + 1 : 1;
            var coverage = Enumerable.Range(0, length)
                .Select(index => _coverage.ContainsKey(index) ? (int?) _coverage[index] : null)
                .ToArray();
            return new CoverageFile(_filePath, new string[0], coverage);
        }
    }
}
