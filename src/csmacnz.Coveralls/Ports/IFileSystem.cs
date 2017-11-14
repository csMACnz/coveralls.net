using System.IO;
using Beefeater;

namespace csmacnz.Coveralls.Ports
{
    public interface IFileSystem: IFileStorer, IFileLoader
    {
    }

    public interface IFileStorer
    {
        bool WriteFile(string outputFile, string fileData);
    }

    public interface IFileLoader
    {
        Option<FileInfo[]> GetFiles(string directory);
        Option<string[]> TryReadAllLinesFromFile(string filePath);
        Option<string> TryLoadFile(string filePath);
    }
}