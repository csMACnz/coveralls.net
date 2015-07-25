using Beefeater;

namespace csmacnz.Coveralls
{
    public interface IFileSystem
    {
        Option<string> TryLoadFile(string filePath);
    }
}