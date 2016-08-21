using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace csmacnz.Coveralls
{
    public class DynamicCodeCoverageParser
    {
        public List<FileCoverageData> GenerateSourceFiles(XDocument document)
        {
            var files = new List<FileCoverageData>();
            var xElement = document.Root?.Element("modules");
            if (xElement != null)
                foreach (var module in xElement.Elements("module"))
                {
                    var filesElement = module.Element("source_files");
                    if (filesElement != null)
                    {
                        foreach (var file in filesElement.Elements("source_file"))
                        {
                            var fileid = file.Attribute("id").Value;
                            var fullPath = file.Attribute("path").Value;

                            var coverageBuilder = new FileCoverageDataBuilder(fullPath);

                            var classesElement = module.Element("functions");
                            if (classesElement != null)
                            {
                                foreach (var @class in classesElement.Elements("function"))
                                {
                                    var ranges = @class.Element("ranges");
                                    if (ranges != null)
                                    {
                                        foreach (var range in ranges.Elements("range"))
                                        {
                                            var rangeFileId = range.Attribute("source_id").Value;
                                            if (fileid == rangeFileId)
                                            {
                                                var sourceStartLine = int.Parse(range.Attribute("start_line").Value);
                                                var sourceEndLine = int.Parse(range.Attribute("end_line").Value);
                                                var covered = range.Attribute("covered").Value == "yes";

                                                foreach (
                                                    var lineNumber in
                                                        Enumerable.Range(sourceStartLine,
                                                            sourceEndLine - sourceStartLine + 1))
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
            return files;
        }
    }
}