using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace csmacnz.Coveralls.Parsers
{
    public class DynamicCodeCoverageParser : IParser
    {
        private readonly PathProcessor _pathProcessor;
        private readonly IFileSystem _fileSystem;

        public DynamicCodeCoverageParser(PathProcessor pathProcessor, IFileSystem fileSystem)
        {
            _pathProcessor = pathProcessor;
            _fileSystem = fileSystem;
        }

        public List<CoverageFile> GenerateSourceFiles(string filePath, bool useRelativePaths)
        {
            var document = _fileSystem.LoadDocument(filePath);
            if (document.Root == null)
            {
                return new List<CoverageFile>();
            }
            var xElement = document.Root.Element("modules");
            if (xElement == null)
            {
                return new List<CoverageFile>();
            }
            var files = new List<FileCoverageData>();
            foreach (var module in xElement.Elements("module"))
            {
                var filesElement = module.Element("source_files");
                if (filesElement == null)
                {
                    continue;
                }
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
                            if (ranges == null)
                            {
                                continue;
                            }
                            foreach (var range in ranges.Elements("range"))
                            {
                                var rangeFileId = range.Attribute("source_id").Value;
                                if (fileid != rangeFileId)
                                {
                                    continue;
                                }
                                var sourceStartLine = int.Parse(range.Attribute("start_line").Value);
                                var sourceEndLine = int.Parse(range.Attribute("end_line").Value);
                                var covered = range.Attribute("covered").Value == "yes";

                                foreach (var lineNumber in Enumerable.Range(sourceStartLine, sourceEndLine - sourceStartLine + 1))
                                {
                                    coverageBuilder.RecordCoverage(lineNumber, covered ? 1 : 0);
                                }
                            }
                        }
                    }
                    files.Add(coverageBuilder.CreateFile());
                }
            }
            var fileCoverageDataConverter = new FileCoverageDataConverter(_pathProcessor, _fileSystem);
            return fileCoverageDataConverter.Convert(files, useRelativePaths);
        }
    }
}