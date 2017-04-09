using System.Collections.Generic;
using System.Linq;
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

    public class ChutzpahJsonParser
    {
        private readonly PathProcessor _pathProcessor;

        public ChutzpahJsonParser(PathProcessor pathProcessor)
        {
            _pathProcessor = pathProcessor;
        }

        public List<CoverageFile> GenerateSourceFiles(string content, bool useRelativePaths)
        {
            var files = new List<CoverageFile>();

            var deserializedString = JsonConvert.DeserializeObject<dynamic>(content);

            foreach (var file in deserializedString)
            {
                ChutzpahJsonFileItem item = JsonConvert.DeserializeObject<ChutzpahJsonFileItem>(file.Value.ToString());

                var currentFilePath = item.FilePath;

                if (item.LineExecutionCounts.Length == item.SourceLines.Length + 1)
                {
                    item.LineExecutionCounts = item.LineExecutionCounts.Skip(1).ToArray(); // fix chutzpah issue.
                }

                if (useRelativePaths)
                {
                    currentFilePath = _pathProcessor.ConvertPath(currentFilePath);
                }

                currentFilePath = _pathProcessor.UnixifyPath(currentFilePath);

                files.Add(new CoverageFile(currentFilePath,
                    Crypto.CalculateMD5Digest(string.Join(",", item.SourceLines)), item.LineExecutionCounts));
            }

            return files;
        }
    }
}