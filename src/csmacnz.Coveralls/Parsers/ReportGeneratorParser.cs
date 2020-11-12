using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using csmacnz.Coveralls.Data;

namespace csmacnz.Coveralls.Parsers
{
    public static class ReportGeneratorParser
    {
        public static List<FileCoverageData> GenerateSourceFiles(Dictionary<string, XDocument> documents)
        {
            _ = documents ?? throw new ArgumentNullException(nameof(documents));

            var files = new List<FileCoverageData>();
            foreach (var fileName in documents.Keys.Where(k => k != "Summary.xml"))
            {
                var rootDocument = documents[fileName];
                var filesElement = rootDocument.Root?.Element(XName.Get("Files"));
                if (filesElement != null)
                {
                    foreach (var fileElement in filesElement.Elements("File"))
                    {
                        var filePath = fileElement.Attribute(XName.Get("name")) !.Value;

                        var source = new List<string>();
                        var coverage = new List<int?>();
                        foreach (var lineAnalysis in fileElement.Elements("LineAnalysis"))
                        {
                            var visits = lineAnalysis.Attribute(XName.Get("visits")) !.Value;
                            var coverageMode = lineAnalysis.Attribute(XName.Get($"coverage")) !.Value;
                            var content = lineAnalysis.Attribute(XName.Get("content")) !.Value;

                            source.Add(WebUtility.HtmlDecode(content));

                            int? coverageCount = null;
                            if (coverageMode == "Covered" || coverageMode == "NotCovered")
                            {
                                var actualVisits = int.Parse(visits, CultureInfo.InvariantCulture);
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
