using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using csmacnz.Coveralls.Data;

namespace csmacnz.Coveralls.Parsers
{
    public class ReportGeneratorParser
    {
        private readonly PathProcessor _pathProcessor;

        public ReportGeneratorParser(PathProcessor pathProcessor)
        {
            _pathProcessor = pathProcessor;
        }

        public List<CoverageFile> GenerateSourceFiles(Dictionary<string, XDocument> documents, bool useRelativePaths)
        {
            var sourceFiles = new List<CoverageFile>();
            foreach (var fileName in documents.Keys.Where(k => k != "Summary.xml"))
            {
            }

            return sourceFiles;
        }
}