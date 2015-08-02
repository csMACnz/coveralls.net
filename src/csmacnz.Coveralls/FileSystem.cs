using System.IO;
using Beefeater;

namespace csmacnz.Coveralls
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
    }
}