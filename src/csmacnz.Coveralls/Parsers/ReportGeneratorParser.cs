using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using csmacnz.Coveralls.Data;
using System.Net;

namespace csmacnz.Coveralls.Parsers
{
    public static class ReportGeneratorParser
    {
        public static List<FileCoverageData> GenerateSourceFiles(Dictionary<string, XDocument> documents)
        {
            var files = new List<FileCoverageData>();
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

                            source.Add(WebUtility.HtmlDecode(content));

                            int? coverageCount = null;
                            if (coverageMode == "Covered" || coverageMode == "NotCovered")
                            {
                                var actualVisits = int.Parse(visits);
                                if (actualVisits != -1)
                                {
                                    coverageCount = actualVisits;
                                }
                            }
                            coverage.Add(coverageCount);
                        }

                        var coverageData = new FileCoverageData(filePath, coverage.ToArray(), source.ToArray());

                        files.Add(coverageData);
                    }
                }
            }
            return files;
        }
    }
}