using System.Collections.Generic;
using System.Linq;
using csmacnz.Coveralls.DataAccess;
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

    public class ChutzpahJsonParser : IParser
    {
        private readonly PathProcessor _pathProcessor;
        private readonly IFileSystem _fileSystem;
        public ChutzpahJsonParser(PathProcessor pathProcessor, IFileSystem fileSystem)
        {
            _pathProcessor = pathProcessor;
            _fileSystem = fileSystem;
        }

        public List<CoverageFile> GenerateSourceFiles(string filePath, bool useRelativePaths)
        {
            var files = new List<CoverageFile>();
            var fileContent = _fileSystem.ReadAllText(filePath);
            var deserializedString = JsonConvert.DeserializeObject<dynamic>(fileContent);
            foreach (var file in deserializedString)
            {
                ChutzpahJsonFileItem item = JsonConvert.DeserializeObject<ChutzpahJsonFileItem>(file.Value.ToString());
                string currentFilePath = item.FilePath;
                if (item.LineExecutionCounts.Length == item.SourceLines.Length + 1)
                {
                    item.LineExecutionCounts = item.LineExecutionCounts.Skip(1).ToArray(); // fix chutzpah issue.
                }
                currentFilePath = _fileSystem.GetCorrectCaseOfParentFolder(currentFilePath); // avoid issue with lower case chutzpah characters
                if (useRelativePaths)
                {
                    currentFilePath = _pathProcessor.ConvertPath(currentFilePath);
                    currentFilePath = _pathProcessor.UnixifyPath(currentFilePath);
                }
                files.Add(new CoverageFile(currentFilePath, Crypto.CalculateMD5Digest(string.Join(",", item.SourceLines)), item.LineExecutionCounts));
            }
            return files;
        }

        
    }
}
