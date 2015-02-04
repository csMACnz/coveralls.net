using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace csmacnz.Coveralls
{
    public class CoverageFileBuilder
    {
        private readonly string _filePath;
        private readonly Dictionary<int,int> _coverage = new Dictionary<int, int>();
        private List<string> _sourceLines;

        public CoverageFileBuilder(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("filePath");
            }
            _filePath = filePath;
        }

        public void AddSource(string source)
        {
            var lines = new List<string>();
            using (var sr = new StringReader(source))
            {
                string nextLine;
                while ((nextLine = sr.ReadLine()) != null)
                {
                    lines.Add(nextLine);
                }
            }
            _sourceLines = lines;
        }

        public void RecordCoverage(int lineNumber, int coverageNumber)
        {
            _coverage[lineNumber - 1] = coverageNumber;
        }

        public CoverageFile CreateFile()
        {
            var length = _sourceLines != null ? _sourceLines.Count : _coverage.Any() ? _coverage.Max(c => c.Key) + 1 : 1;
            var coverage = Enumerable.Range(0, length)
                .Select(index => _coverage.ContainsKey(index) ? (int?) _coverage[index] : null)
                .ToArray();
            var sourceLines = _sourceLines != null ? _sourceLines.ToArray() : new string[0];
            return new CoverageFile(_filePath, sourceLines, coverage);
        }
    }
}
