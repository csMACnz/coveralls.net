using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace csmacnz.Coveralls.Parsers
{
    public class MonoCoverParser : IParser
    {
        private readonly PathProcessor _pathProcessor;
        private readonly IFileSystem _fileSystem;

        public MonoCoverParser(PathProcessor pathProcessor, IFileSystem fileSystem)
        {
            _pathProcessor = pathProcessor;
            _fileSystem = fileSystem;
        }

        public List<CoverageFile> GenerateSourceFiles(string folder, bool useRelativePaths)
        {
            Dictionary<string, XDocument> documents =
                    new DirectoryInfo(folder).GetFiles()
                        .Where(f => f.Name.EndsWith(".xml"))
                        .ToDictionary(f => f.Name, f => _fileSystem.LoadDocument(f.FullName));

            var sourceFiles = new List<CoverageFile>();
            foreach (var fileName in documents.Keys.Where(k => k.StartsWith("class-") && k.EndsWith(".xml")))
            {
                var rootDocument = documents[fileName];
                if (rootDocument.Root != null)
                {
                    var sourceElement = rootDocument.Root.Element("source");
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
            }


            return sourceFiles;
        }
    }
}