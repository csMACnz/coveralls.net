﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using csmacnz.Coveralls.Data;

namespace csmacnz.Coveralls.Parsers
{
    public static class ExportCodeCoverageParser
    {
        public static List<FileCoverageData> GenerateSourceFiles(XDocument document)
        {
            _ = document ?? throw new ArgumentNullException(nameof(document));

            var files = new List<FileCoverageData>();
            if (document.Root != null)
            {
                var sourceFilesInfo = new Dictionary<string, string>();

                foreach (var sourceFile in document.Root.Elements("SourceFileNames"))
                {
                    var idElement = sourceFile.Element(XName.Get("SourceFileID"));

                    if (idElement == null)
                    {
                        continue;
                    }

                    var id = idElement.Value;

                    var fileNameElement = sourceFile.Element(XName.Get("SourceFileName"));

                    if (fileNameElement == null)
                    {
                        continue;
                    }

                    var fileName = fileNameElement.Value;

                    sourceFilesInfo.Add(id, fileName);
                }

                foreach (var module in document.Root.Elements("Module"))
                {
                    foreach (var sourceFileInfo in sourceFilesInfo)
                    {
                        var fileId = sourceFileInfo.Key;
                        var fullPath = sourceFileInfo.Value;

                        var coverageBuilder = new FileCoverageDataBuilder(fullPath);

                        var namespaceTable = module.Element(XName.Get("NamespaceTable"));
                        if (namespaceTable == null)
                        {
                            continue;
                        }

                        foreach (var @class in namespaceTable.Elements("Class"))
                        {
                            foreach (var method in @class.Elements("Method"))
                            {
                                foreach (var lines in method.Elements("Lines"))
                                {
                                    var sourceFileIdElement = lines.Element(XName.Get("SourceFileID"));

                                    if (sourceFileIdElement == null)
                                    {
                                        continue;
                                    }

                                    var sourceFileId = sourceFileIdElement.Value;

                                    if (sourceFileId != fileId)
                                    {
                                        continue;
                                    }

                                    var sourceStartLineElement = lines.Element(XName.Get("LnStart"));

                                    if (sourceStartLineElement == null)
                                    {
                                        continue;
                                    }

                                    var sourceStartLine = int.Parse(sourceStartLineElement.Value, CultureInfo.InvariantCulture);

                                    var sourceEndLineElement = lines.Element(XName.Get("LnEnd"));

                                    if (sourceEndLineElement == null)
                                    {
                                        continue;
                                    }

                                    var sourceEndLine = int.Parse(sourceEndLineElement.Value, CultureInfo.InvariantCulture);

                                    var coveredElement = lines.Element(XName.Get("Coverage"));

                                    if (coveredElement == null)
                                    {
                                        continue;
                                    }

                                    // A value of 2 means completely covered
                                    var covered = coveredElement.Value == "2";

                                    for (var lineNumber = sourceStartLine; lineNumber <= sourceEndLine; lineNumber++)
                                    {
                                        coverageBuilder.RecordCoverage(lineNumber, covered ? 1 : 0);
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
