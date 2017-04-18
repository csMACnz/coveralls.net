using System.IO;
using System.Xml.Linq;
using Beefeater;

namespace csmacnz.Coveralls.Ports
{
    public interface IFileSystem
    {
        Option<string> TryLoadFile(string filePath);
        Option<XDocument> TryLoadXDocumentFromFile(string filePath);
        Option<FileInfo[]> GetFiles(string directory);
        bool WriteFile(string outputFile, string fileData);
        Option<string[]> TryReadAllLinesFromFile(string filePath);
    }
}