using System.Collections.Generic;
using System.Xml.Linq;

namespace csmacnz.Coveralls
{
    public class VSCodeCoverageParser
    {
        private readonly FileSystem _fileSystem;
        private readonly PathProcessor _pathProcessor;

        public VSCodeCoverageParser(FileSystem fileSystem, PathProcessor pathProcessor)
        {
            _fileSystem = fileSystem;
            _pathProcessor = pathProcessor;
        }

        public List<CoverageFile> GenerateSourceFiles(XDocument document, bool useRelativePaths)
        {
            return new List<CoverageFile>();
        }
    }
}