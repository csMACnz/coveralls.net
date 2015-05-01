using System.IO;

namespace csmacnz.Coveralls
{
    public class PathProcessor
    {
        private readonly string _basePath;

        public PathProcessor(string basePath)
        {
            _basePath = !string.IsNullOrWhiteSpace(basePath) ? basePath : Directory.GetCurrentDirectory();
        }

        public string ConvertPath(string path)
        {
            var currentWorkingDirectory = _basePath.ToLower();

            if (path.ToLower().StartsWith(currentWorkingDirectory))
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