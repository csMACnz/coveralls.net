using System.IO;

namespace csmacnz.Coveralls
{
    public class PathProcessor
    {
        public string ConvertPath(string path)
        {
            var currentWorkingDirectory = Directory.GetCurrentDirectory();
            if (path.StartsWith(currentWorkingDirectory))
            {
                return path.Substring(currentWorkingDirectory.Length);
            }
            return path;
        }


        public string UnixifyPath(string filePath)
        {
            return filePath.Replace('\\', '/');
        }
    }
}