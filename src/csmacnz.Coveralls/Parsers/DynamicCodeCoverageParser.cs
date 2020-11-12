using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using csmacnz.Coveralls.Data;

namespace csmacnz.Coveralls.Parsers
{
    public static class DynamicCodeCoverageParser
    {
        public static List<FileCoverageData> GenerateSourceFiles(XDocument document)
        {
            _ = document ?? throw new ArgumentNullException(nameof(document));

            var files = new List<FileCoverageData>();
            var xElement = document.Root?.Element(XName.Get("modules"));
            if (xElement != null)
            {
                foreach (var module in xElement.Elements("module"))
                {
                    var filesElement = module.Element(XName.Get("source_files"));
                    if (filesElement != null)
                    {
                        foreach (var file in filesElement.Elements("source_file"))
                        {
                            var fileid = file.Attribute(XName.Get("id")) !.Value;
                            var fullPath = file.Attribute(XName.Get("path")) !.Value;

                            var coverageBuilder = new FileCoverageDataBuilder(fullPath);

                            var classesElement = module.Element(XName.Get("functions"));
                            if (classesElement != null)
                            {
                                foreach (var @class in classesElement.Elements("function"))
                                {
                                    var ranges = @class.Element(XName.Get("ranges"));
                                    if (ranges != null)
                                    {
                                        foreach (var range in ranges.Elements("range"))
                                        {
                                            var rangeFileId = range.Attribute(XName.Get("source_id")) !.Value;
                                            if (fileid == rangeFileId)
                                            {
                                                var sourceStartLine = int.Parse(range.Attribute(XName.Get("start_line")) !.Value, CultureInfo.InvariantCulture);
                                                var sourceEndLine = int.Parse(range.Attribute(XName.Get("end_line")) !.Value, CultureInfo.InvariantCulture);
                                                var covered = range.Attribute(XName.Get("covered")) !.Value == "yes";

                                                var sourceLineNumbers = Enumerable.Range(
                                                    sourceStartLine,
                                                    sourceEndLine - sourceStartLine + 1);

                                                foreach (var lineNumber in sourceLineNumbers)
                                                {
                                                    coverageBuilder.RecordCoverage(lineNumber, covered ? 1 : 0);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            files.Add(coverageBuilder.CreateFile());
                        }
                    }
                }
            }

            return files;
        }
    }
}
