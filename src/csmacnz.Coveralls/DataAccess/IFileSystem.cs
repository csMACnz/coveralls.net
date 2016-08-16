using System.Xml.Linq;
using Beefeater;

namespace csmacnz.Coveralls
{
    public interface IFileSystem
    {
        Option<string> TryLoadFile(string filePath);
        string ReadAllText(string filePath);
        string GetCorrectCaseOfParentFolder(string fileOrFolder);

        XDocument LoadDocument(string filePath);
    }
}