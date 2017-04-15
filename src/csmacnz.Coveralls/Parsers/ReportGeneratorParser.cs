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
                var rootDocument = documents[fileName];
                var filesElement = rootDocument.Root?.Element("Files");
                if (filesElement != null)
                {

                    foreach (var fileElement in filesElement.Elements("File"))
                    {
                        var filePath = fileElement.Attribute("name").Value;
                        var source = new List<string>();
                        var coverage = new List<int?>();
                        foreach (var lineAnalysis in fileElement.Elements("LineAnalysis"))
                        {
                            var line = lineAnalysis.Attribute("line").Value;
                            var visits = lineAnalysis.Attribute("visits").Value;
                            var coverageMode = lineAnalysis.Attribute("coverage").Value;
                            var coveredbranches = lineAnalysis.Attribute("coveredbranches").Value;
                            var totalbranches = lineAnalysis.Attribute("totalbranches").Value;
                            var content = lineAnalysis.Attribute("content").Value;

                            source.Add(content);
                            coverage.Add(null);
                        }

                        var sourceDigest = Crypto.CalculateMD5Digest(string.Join(",", source.ToArray()));
                        var coverageFile = new CoverageFile(filePath, sourceDigest, coverage.ToArray());
                        sourceFiles.Add(coverageFile);
                    }
                }
            }
            return sourceFiles;
        }
    }
}