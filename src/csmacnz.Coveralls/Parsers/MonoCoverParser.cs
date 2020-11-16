using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using csmacnz.Coveralls.Data;

namespace csmacnz.Coveralls.Parsers
{
    public static class MonoCoverParser
    {
        public static List<FileCoverageData> GenerateSourceFiles(Dictionary<string, XDocument> documents)
        {
            _ = documents ?? throw new ArgumentNullException(nameof(documents));

            var sourceFiles = new List<FileCoverageData>();
            foreach (var fileName in documents.Keys.Where(IsMonoCoverageClassFileName))
            {
                var rootDocument = documents[fileName];
                var sourceElement = rootDocument.Root?.Element("source");
                if (sourceElement != null)
                {
                    var coverage = new List<int?>();
                    var source = new List<string>();
                    var filePath = sourceElement.Attribute("sourceFile")!.Value;

                    foreach (var line in sourceElement.Elements("l"))
                    {
                        if (!int.TryParse(line.Attribute("count")!.Value, out var coverageCount))
                        {
                            coverageCount = -1;
                        }

                        coverage.Add(coverageCount == -1 ? null : (int?)coverageCount);
                        source.Add(line.Value);
                    }

                    sourceFiles.Add(new FileCoverageData(filePath, coverage.ToArray(), source.ToArray()));
                }
            }

            return sourceFiles;
        }

        private static bool IsMonoCoverageClassFileName(string k)
        {
            return k.StartsWith("class-", StringComparison.Ordinal) && k.EndsWith(".xml", StringComparison.Ordinal);
        }
    }
}
