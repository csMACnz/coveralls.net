namespace csmacnz.Coveralls.Parsers;

public static class OpenCoverParser
{
    public static List<FileCoverageData> GenerateSourceFiles(XDocument document)
    {
        _ = document ?? throw new ArgumentNullException(nameof(document));

        var files = new List<FileCoverageData>();
        var xElement = document.Root?.Element("Modules");
        if (xElement != null)
        {
            foreach (var module in xElement.Elements("Module"))
            {
                var attribute = module.Attribute("skippedDueTo");
                if (string.IsNullOrEmpty(attribute?.Value))
                {
                    var filesElement = module.Element("Files");
                    if (filesElement != null)
                    {
                        foreach (var file in filesElement.Elements("File"))
                        {
                            var fileid = file.Attribute("uid")!.Value;
                            var fullPath = file.Attribute("fullPath")!.Value;

                            var coverageBuilder = new FileCoverageDataBuilder(fullPath);

                            var classesElement = module.Element("Classes");
                            if (classesElement != null)
                            {
                                foreach (var @class in classesElement.Elements("Class"))
                                {
                                    var methods = @class.Element("Methods");
                                    if (methods != null)
                                    {
                                        foreach (var method in methods.Elements("Method"))
                                        {
                                            var sequencePointsElement = method.Element("SequencePoints");
                                            if (sequencePointsElement != null)
                                            {
                                                foreach (
                                                    var sequencePoint in
                                                        sequencePointsElement.Elements("SequencePoint"))
                                                {
                                                    var sequenceFileid = sequencePoint.Attribute($"fileid")!.Value;
                                                    if (fileid == sequenceFileid)
                                                    {
                                                        var sourceLine =
                                                            int.Parse(sequencePoint.Attribute("sl")!.Value, CultureInfo.InvariantCulture);
                                                        var visitCount =
                                                            int.Parse(sequencePoint.Attribute("vc")!.Value, CultureInfo.InvariantCulture);

                                                        coverageBuilder.RecordCoverage(sourceLine, visitCount);
                                                    }
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
        }

        return files;
    }
}
