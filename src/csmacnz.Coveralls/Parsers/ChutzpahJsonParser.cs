using System;
using System.Collections.Generic;
using System.Linq;
using csmacnz.Coveralls.Data;
using Newtonsoft.Json;

namespace csmacnz.Coveralls.Parsers
{
    public class ChutzpahJsonFileItem
    {
        public string FilePath { get; set; }

        public int?[] LineExecutionCounts { get; set; }

        public string[] SourceLines { get; set; }

        public double CoveragePercentage { get; set; }
    }

    public static class ChutzpahJsonParser
    {
        public static List<FileCoverageData> GenerateSourceFiles(string[] content)
        {
            var files = new List<FileCoverageData>();

            var deserializedString = JsonConvert.DeserializeObject<dynamic>(string.Join(Environment.NewLine, content));

            foreach (var file in deserializedString)
            {
                ChutzpahJsonFileItem item = JsonConvert.DeserializeObject<ChutzpahJsonFileItem>(file.Value.ToString());

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