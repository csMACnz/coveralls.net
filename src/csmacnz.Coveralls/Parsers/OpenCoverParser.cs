using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using csmacnz.Coveralls.Data;

namespace csmacnz.Coveralls.Parsers
{
    public static class OpenCoverParser
    {
        public static List<FileCoverageData> GenerateSourceFiles(XDocument document)
        {
            _ = document ?? throw new ArgumentNullException(nameof(document));

            var files = new List<FileCoverageData>();
            var xElement = document.Root?.Element(XName.Get("Modules"));
            if (xElement != null)
            {
                foreach (var module in xElement.Elements("Module"))
                {
                    var attribute = module.Attribute(XName.Get("skippedDueTo")) !;
                    if (string.IsNullOrEmpty(attribute?.Value))
                    {
                        var filesElement = module.Element(XName.Get("Files"));
                        if (filesElement != null)
                        {
                            foreach (var file in filesElement.Elements("File"))
                            {
                                var fileid = file.Attribute(XName.Get("uid")) !.Value;
                                var fullPath = file.Attribute(XName.Get("fullPath")) !.Value;

                                var coverageBuilder = new FileCoverageDataBuilder(fullPath);

                                var classesElement = module.Element(XName.Get("Classes"));
                                if (classesElement != null)
                                {
                                    foreach (var @class in classesElement.Elements("Class"))
                                    {
                                        var methods = @class.Element(XName.Get("Methods"));
                                        if (methods != null)
                                        {
                                            foreach (var method in methods.Elements("Method"))
                                            {
                                                var sequencePointsElement = method.Element(XName.Get("SequencePoints"));
                                                if (sequencePointsElement != null)
                                                {
                                                    foreach (
                                                        var sequencePoint in
                                                            sequencePointsElement.Elements("SequencePoint"))
                                                    {
                                                        var sequenceFileid = sequencePoint.Attribute(XName.Get($"fileid")) !.Value;
                                                        if (fileid == sequenceFileid)
                                                        {
                                                            var sourceLine =
                                                                int.Parse(sequencePoint.Attribute(XName.Get("sl")) !.Value, CultureInfo.InvariantCulture);
                                                            var visitCount =
                                                                int.Parse(sequencePoint.Attribute(XName.Get("vc")) !.Value, CultureInfo.InvariantCulture);

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
}
