using System.IO;

namespace csmacnz.Coveralls
{
    public class FileSystem : IFileSystem
    {
        public string TryLoadFile(string fullPath)
        {
            return File.ReadAllText(fullPath);
        }
    }
}