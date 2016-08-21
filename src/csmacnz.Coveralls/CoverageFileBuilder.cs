using System.Collections.Generic;
using System.IO;
using BCLExtensions;

namespace csmacnz.Coveralls
{
    public class CoverageFileBuilder
    {
        private readonly int?[] _coverage;
        private string _filePath;
        private List<string> _sourceLines;

        public CoverageFileBuilder(FileCoverageData data)
        {
            data.EnsureIsNotNull("data");
            _coverage = data.Coverage;
            _filePath = data.FullPath;
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

        public void SetPath(string path)
        {
            _filePath = path;
        }

        public CoverageFile CreateFile()
        {
            var length = _sourceLines?.Count ?? _coverage.Length;
            var coverage = _coverage;
            if (length > _coverage.Length)
            {
                coverage = new int?[length];
                _coverage.CopyTo(coverage, 0);
            }

            var sourceDigest = Crypto.CalculateMD5Digest(string.Join("\n", _sourceLines?.ToArray() ?? new string[0]));
            return new CoverageFile(_filePath, sourceDigest, coverage);
        }
    }
}