using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace csmacnz.Coveralls
{
    public class OpenCoverParser
    {
        private readonly IFileSystem _fileSystem;

        public OpenCoverParser(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public List<CoverageFile> GenerateSourceFiles(XDocument document)
        {
            var files = new List<CoverageFile>();
            if (document.Root != null)
            {
                var xElement = document.Root.Element("Modules");
                if (xElement != null)
                    foreach (var module in xElement.Elements("Module"))
                    {
                        var attribute = module.Attribute("skippedDueTo");
                        if (attribute == null || string.IsNullOrEmpty(attribute.Value))
                        {
                            var filesElement = module.Element("Files");
                            if (filesElement != null)
                            {
                                foreach (var file in filesElement.Elements("File"))
                                {
                                    var fileid = file.Attribute("uid").Value;
                                    var fullPath = file.Attribute("fullPath").Value;
                                    var compatibleFilePath = UnixifyPath(fullPath);
                                    var coverageBuilder = new CoverageFileBuilder(compatibleFilePath);

                                    var classesElement = module.Element("Classes");
                                    if (classesElement != null)
                                    {
                                        foreach (var @class in classesElement.Elements("Class"))
                                        {
                                            var methods = @class.Element("Methods");
                                            if (methods != null)
                                                foreach (var method in methods.Elements("Method"))
                                                {
                                                    var sequencePointsElement = method.Element("SequencePoints");
                                                    if (sequencePointsElement != null)
                                                        foreach (var sequencePoint in sequencePointsElement.Elements("SequencePoint"))
                                                        {
                                                            var sequenceFileid = sequencePoint.Attribute("fileid").Value;
                                                            if (fileid == sequenceFileid)
                                                            {
                                                                var sourceLine = int.Parse(sequencePoint.Attribute("sl").Value);
                                                                var visitCount = int.Parse(sequencePoint.Attribute("vc").Value);

                                                                coverageBuilder.RecordCoverage(sourceLine, visitCount);
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
            }
            return files;
        }

        private string UnixifyPath(string fullPath)
        {
            return fullPath.Replace(Path.DirectorySeparatorChar, '/');
        }
    }
}
