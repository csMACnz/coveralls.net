using System.IO;

namespace csmacnz.Coveralls
{
    public class FileSystem : IFileSystem
    {
        public string TryLoadFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
            return null;
        }
    }
}