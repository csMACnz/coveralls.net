using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace csmacnz.Coveralls
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

        public List<CoverageFile> GenerateSourceFiles(string fileContent, bool useRelativePaths)
        {
            var files = new List<CoverageFile>();
            var deserializedString = JsonConvert.DeserializeObject<dynamic>(fileContent);
            foreach (var file in deserializedString)
            {
                ChutzpahJsonFileItem item = JsonConvert.DeserializeObject<ChutzpahJsonFileItem>(file.Value.ToString());
                string filePath = item.FilePath;
                if (item.LineExecutionCounts.Length == item.SourceLines.Length + 1)
                {
                    item.LineExecutionCounts = item.LineExecutionCounts.Skip(1).ToArray(); // fix chutzpah issue.
                }
                if (useRelativePaths)
                {
                    filePath = GetCorrectCaseOfParentFolder(filePath); // avoid issue with lower case chutzpah characters
                    filePath = _pathProcessor.ConvertPath(filePath);
                    filePath = _pathProcessor.UnixifyPath(filePath);
                }
                files.Add(new CoverageFile(filePath, Crypto.CalculateMD5Digest(string.Join(",", item.SourceLines)), item.LineExecutionCounts));
            }
            return files;
        }

        private static string GetCorrectCaseOfParentFolder(string fileOrFolder)
        {
            var myParentFolder = Path.GetDirectoryName(fileOrFolder);
            var myChildName = Path.GetFileName(fileOrFolder);
            if (myParentFolder == null)
            {
                return fileOrFolder.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            }
            if (!Directory.Exists(myParentFolder))
            {
                return GetCorrectCaseOfParentFolder(myParentFolder) + Path.DirectorySeparatorChar + myChildName;
            }
            string myFileOrFolder = Directory.GetFileSystemEntries(myParentFolder, myChildName).FirstOrDefault();
            if (myFileOrFolder != null)
            {
                myChildName = Path.GetFileName(myFileOrFolder);
            }
            return GetCorrectCaseOfParentFolder(myParentFolder) + Path.DirectorySeparatorChar + myChildName;
        }
    }
}
