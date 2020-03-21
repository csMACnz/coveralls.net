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

        public string BasePath { get; } = GenerateRandomAbsolutePath("WorkingDir");

        public Option<string> TryLoadFile(string filePath)
        {
            var pathKey = ResolvePathKey(filePath);
            if (_files.ContainsKey(pathKey))
            {
                return _files[pathKey];
            }

            return Option<string>.None;
        }

        public static string GenerateRandomAbsolutePath(params string[] paths)
        {
            return Path.Combine(Path.GetFullPath(@"\"), Path.GetRandomFileName(), Path.Combine(paths));
        }

        public Option<FileInfo[]> GetFiles(string directory)
        {
            if (_files.Any())
            {
                var directoryAsPathKey = ResolvePathKey(directory);
                return _files
                    .Where(kvp => kvp.Key.StartsWith(directoryAsPathKey, StringComparison.OrdinalIgnoreCase))
                    .Select(kvp => new FileInfo(kvp.Key))
                    .ToArray();
            }

            return Option<FileInfo[]>.None;
        }

        public bool WriteFile(string outputFile, string fileData)
        {
            AddFile(outputFile, fileData);

            return true;
        }

        public Option<string[]> TryReadAllLinesFromFile(string filePath)
        {
            if (_files.ContainsKey(filePath))
            {
                return _files[filePath].Split('\n');
            }

            return Option<string[]>.None;
        }

        public void AddFile(string path, string contents)
        {
            _files[ResolvePathKey(path)] = contents;
        }

        private string ResolvePathKey(string path)
        {
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(BasePath, path);
            }

            return Path.GetFullPath(path);
        }
    }
}
