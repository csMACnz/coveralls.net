using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace csmacnz.Coveralls
{
    public class DynamicCodeCoverageParser
    {
        private readonly FileSystem _fileSystem;
        private readonly PathProcessor _pathProcessor;

        public DynamicCodeCoverageParser(FileSystem fileSystem, PathProcessor pathProcessor)
        {
            _fileSystem = fileSystem;
            _pathProcessor = pathProcessor;
        }

        public List<CoverageFile> GenerateSourceFiles(XDocument document, bool useRelativePaths)
        {
            var files = new List<CoverageFile>();
            if (document.Root != null)
            {
                var xElement = document.Root.Element("modules");
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
                                var path = fullPath;
                                if (useRelativePaths)
                                {
                                    path = _pathProcessor.ConvertPath(fullPath);
                                }
                                path = _pathProcessor.UnixifyPath(path);
                                var coverageBuilder = new CoverageFileBuilder(path);

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

                                var readAllText = _fileSystem.TryLoadFile(fullPath);
                                if (readAllText != null)
                                {
                                    coverageBuilder.AddSource(readAllText);
                                }
                                var coverageFile = coverageBuilder.CreateFile();
                                files.Add(coverageFile);
                            }
                        }
                    }
            }
            return files;
        }
    }
}