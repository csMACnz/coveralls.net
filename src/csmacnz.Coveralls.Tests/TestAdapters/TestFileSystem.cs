using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Beefeater;
using csmacnz.Coveralls.Ports;

namespace csmacnz.Coveralls.Tests.TestAdapters
{
    public class TestFileSystem : IFileSystem
    {
        private readonly Dictionary<string, string> _files = new Dictionary<string, string>();

        public Option<string> TryLoadFile(string filePath)
        {
            if (_files.ContainsKey(filePath))
            {
                return _files[filePath];
            }

            return null;
        }

        public static string GenerateRandomAbsolutePath(params string[] paths)
        {
            return Path.Combine(Path.GetFullPath(@"\"), Path.GetRandomFileName(), Path.Combine(paths));
        }

        public Option<FileInfo[]> GetFiles(string directory)
        {
            if (_files.Any())
            {
                return _files
                    .Where(kvp => kvp.Key.StartsWith(directory, StringComparison.OrdinalIgnoreCase))
                    .Select(kvp => new FileInfo(kvp.Key))
                    .ToArray();
            }

            return null;
        }

        public bool WriteFile(string outputFile, string fileData)
        {
            // todo: configure toggle
            return true;
        }

        public Option<string[]> TryReadAllLinesFromFile(string filePath)
        {
            if (_files.ContainsKey(filePath))
            {
                return _files[filePath].Split('\n');
            }

            return null;
        }

        public void AddFile(string path, string contents)
        {
            _files[path] = contents;
        }
    }
}
