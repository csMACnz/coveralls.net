using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using csmacnz.Coveralls.Data;

namespace csmacnz.Coveralls.Parsers
{
    public static class NCoverParser
    {
        public static List<FileCoverageData> GenerateSourceFiles(XDocument document)
        {
            _ = document ?? throw new ArgumentNullException(nameof(document));

            var coverage = new List<FileCoverageData>();
            var xElement = document.Root;
            if (xElement != null)
            {
                foreach (var module in xElement.Elements("module"))
                {
                    var files = new Dictionary<string, FileCoverageDataBuilder>();

                    foreach (var method in module.Elements("method"))
                    {
                        XAttribute attribute = method.Attribute("excluded");
                        if (!string.IsNullOrEmpty(attribute?.Value) &&
                             bool.Parse(attribute.Value))
                        {
                                continue;
                        }

                        foreach (var seqpnt in method.Elements("seqpnt"))
                        {
                            attribute = method.Attribute("excluded");
                            if (!string.IsNullOrEmpty(attribute?.Value) &&
                                bool.Parse(attribute.Value))
                            {
                                    continue;
                            }

                            var sourceLine =
                                int.Parse(seqpnt.Attribute("line").Value, CultureInfo.InvariantCulture);
                            var visitCount =
                                int.Parse(seqpnt.Attribute("visitcount").Value, CultureInfo.InvariantCulture);
                            var file = seqpnt.Attribute("doc" + "ument").Value; // avoid false positive error CC0021: Use 'nameof(document)'
                            if (!files.ContainsKey(file))
                            {
                                files[file] = new FileCoverageDataBuilder(file);
                            }

                            files[file].RecordCoverage(sourceLine, visitCount);
                        }
                    }

                    coverage.AddRange(files.Values.Select(builder => builder.CreateFile()));
                }
            }

            return coverage;
        }
    }
}
