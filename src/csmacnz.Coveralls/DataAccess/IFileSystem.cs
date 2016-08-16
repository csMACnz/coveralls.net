using System.Xml.Linq;
using Beefeater;

namespace csmacnz.Coveralls.DataAccess
{
    public interface IFileSystem
    {
        Option<string> TryLoadFile(string filePath);
        string ReadAllText(string filePath);
        string GetCorrectCaseOfParentFolder(string fileOrFolder);
        XDocument LoadDocument(string filePath);

        string[] GetFileSystemEntries(string path);
        bool IsFile(string path);
        bool IsDirectory(string path);
    }
}