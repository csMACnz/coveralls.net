using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace csmacnz.Coveralls
{
    public class MonoCoverParser
    {
        private readonly PathProcessor _pathProcessor;

        public MonoCoverParser(PathProcessor pathProcessor)
        {
            _pathProcessor = pathProcessor;
        }

        public List<CoverageFile> GenerateSourceFiles(Dictionary<string, XDocument> documents, bool useRelativePaths)
        {
            var sourceFiles = new List<CoverageFile>();
            foreach (var fileName in documents.Keys.Where(k => k.StartsWith("class-") && k.EndsWith(".xml")))
            {
                var rootDocument = documents[fileName];
                var sourceElement = rootDocument.Root?.Element("source");
                if (sourceElement != null)
                {
                    List<int?> coverage = new List<int?>();
                    List<string> source = new List<string>();
                    var filePath = sourceElement.Attribute("sourceFile").Value;
                    if (useRelativePaths)
                    {
                        filePath = _pathProcessor.ConvertPath(filePath);
                    }

                    foreach (var line in sourceElement.Elements("l"))
                    {
                        int coverageCount;
                        if (!int.TryParse(line.Attribute("count").Value, out coverageCount))
                        {
                            coverageCount = -1;
                        }
                        coverage.Add(coverageCount == -1 ? null : (int?)coverageCount);
                        source.Add(line.Value);
                    }

                    sourceFiles.Add(new CoverageFile(filePath, Crypto.CalculateMD5Digest(string.Join(",", source.ToArray())), coverage.ToArray()));
                }
            }


            return sourceFiles;
        }
    }
}