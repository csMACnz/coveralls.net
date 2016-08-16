using System.Collections.Generic;
using csmacnz.Coveralls.DataAccess;

namespace csmacnz.Coveralls.Parsers
{
	public class OpenCoverParser : IParser
	{
        private readonly PathProcessor _pathProcessor;
        private readonly IFileSystem _fileSystem;

        public OpenCoverParser(PathProcessor pathProcessor, IFileSystem fileSystem)
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
		    var xElement = document.Root.Element("Modules");
		    if (xElement == null)
		    {
                return new List<CoverageFile>();
		    }
            var files = new List<FileCoverageData>();
            foreach (var module in xElement.Elements("Module"))
		    {
		        var attribute = module.Attribute("skippedDueTo");
		        if (attribute != null && !string.IsNullOrEmpty(attribute.Value))
		        {
		            continue;
		        }
		        var filesElement = module.Element("Files");
		        if (filesElement == null)
		        {
		            continue;
		        }
		        foreach (var file in filesElement.Elements("File"))
		        {
		            var fileid = file.Attribute("uid").Value;
		            var fullPath = file.Attribute("fullPath").Value;

		            var coverageBuilder = new FileCoverageDataBuilder(fullPath);

		            var classesElement = module.Element("Classes");
		            if (classesElement != null)
		            {
		                foreach (var @class in classesElement.Elements("Class"))
		                {
		                    var methods = @class.Element("Methods");
		                    if (methods == null)
		                    {
		                        continue;
		                    }
		                    foreach (var method in methods.Elements("Method"))
		                    {
		                        var sequencePointsElement = method.Element("SequencePoints");
		                        if (sequencePointsElement == null)
		                        {
		                            continue;
		                        }
		                        foreach (var sequencePoint in sequencePointsElement.Elements("SequencePoint"))
		                        {
		                            var sequenceFileid = sequencePoint.Attribute("fileid").Value;
		                            if (fileid != sequenceFileid)
		                            {
		                                continue;
		                            }
		                            var sourceLine = int.Parse(sequencePoint.Attribute("sl").Value);
		                            var visitCount = int.Parse(sequencePoint.Attribute("vc").Value);

		                            coverageBuilder.RecordCoverage(sourceLine, visitCount);
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