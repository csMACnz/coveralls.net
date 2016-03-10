﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace csmacnz.Coveralls
{
    public class ExportCodeCoverageParser
    {
        public List<FileCoverageData> GenerateSourceFiles(XDocument document)
        {
            var files = new List<FileCoverageData>();
            if(document.Root != null)
            {
                var sourceFilesInfo = new Dictionary<string, string>();

                foreach(var sourceFile in document.Root.Elements("SourceFileNames"))
                {
                    var id = sourceFile.Element("SourceFileID")?.Value;
                    var fileName = sourceFile.Element("SourceFileName")?.Value;

                    if(id != null && fileName != null)
                    {
                        sourceFilesInfo.Add(id, fileName);
                    }
                }

                foreach(var module in document.Root.Elements("Module"))
                {
                    foreach(var sourceFileInfo in sourceFilesInfo)
                    {
                        var fileId = sourceFileInfo.Key;
                        var fullPath = sourceFileInfo.Value;

                        var coverageBuilder = new FileCoverageDataBuilder(fullPath);

                        var namespaceTable = module.Element("NamespaceTable");
                        if(namespaceTable == null)
                        {
                            continue;
                        }

                        foreach(var @class in namespaceTable.Elements("Class"))
                        {
                            foreach(var method in @class.Elements("Method"))
                            {
                                foreach(var lines in method.Elements("Lines"))
                                {
                                    var sourceFileId = lines.Element("SourceFileID")?.Value;

                                    if(sourceFileId == null)
                                    {
                                        continue;
                                    }

                                    if(sourceFileId != fileId)
                                    {
                                        continue;
                                    }

                                    var sourceStartLineElement = lines.Element("LnStart");

                                    if(sourceStartLineElement == null)
                                    {
                                        continue;
                                    }

                                    var sourceStartLine = int.Parse(sourceStartLineElement.Value);

                                    var sourceEndLineElement = lines.Element("LnEnd");

                                    if(sourceEndLineElement == null)
                                    {
                                        continue;
                                    }

                                    var sourceEndLine = int.Parse(sourceEndLineElement.Value);


                                    var coveredElement = lines.Element("Coverage");

                                    if(coveredElement == null)
                                    {
                                        continue;
                                    }

                                    // A value of 2 means completely covered
                                    var covered = coveredElement.Value == "2";

                                    for(int lineNumber = sourceStartLine; lineNumber <= sourceEndLine; lineNumber++)
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