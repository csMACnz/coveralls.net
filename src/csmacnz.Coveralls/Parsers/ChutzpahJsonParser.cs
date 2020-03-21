using System;
using System.Collections.Generic;
using System.Linq;
using csmacnz.Coveralls.Data;
using Newtonsoft.Json;

namespace csmacnz.Coveralls.Parsers
{
    public static class ChutzpahJsonParser
    {
        public static List<FileCoverageData> GenerateSourceFiles(string[] content)
        {
            var files = new List<FileCoverageData>();

            var jsonFileContents = JsonConvert.DeserializeObject<Dictionary<string, ChutzpahJsonFileItem>>(string.Join(Environment.NewLine, content));

            foreach (ChutzpahJsonFileItem item in jsonFileContents.Values)
            {
                var currentFilePath = item.FilePath;

                if (item.LineExecutionCounts.Length == item.SourceLines.Length + 1)
                {
                    item.LineExecutionCounts = item.LineExecutionCounts.Skip(1).ToArray(); // fix chutzpah issue.
                }

                files.Add(new FileCoverageData(currentFilePath, item.LineExecutionCounts, item.SourceLines));
            }

            return files;
        }
    }
}
