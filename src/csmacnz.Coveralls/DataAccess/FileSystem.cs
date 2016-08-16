using System.IO;
using System.Linq;
using System.Xml.Linq;
using Beefeater;

namespace csmacnz.Coveralls.DataAccess
{
    public class FileSystem : IFileSystem
    {
        public Option<string> TryLoadFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
            return null;
        }

        public string ReadAllText(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        public string GetCorrectCaseOfParentFolder(string fileOrFolder)
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

        public XDocument LoadDocument(string filePath)
        {
            return XDocument.Load(filePath);
        }

        public string[] GetFileSystemEntries(string path)
        {
            return Directory.GetFileSystemEntries(path);
        }

        public bool IsFile(string path)
        {
            return File.Exists(path);
        }

        public bool IsDirectory(string path)
        {
            return Directory.Exists(path);
        }
    }
}