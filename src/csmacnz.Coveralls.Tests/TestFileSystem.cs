using System;
using System.Collections.Generic;
using System.IO;
using Beefeater;
using csmacnz.Coveralls.Ports;

namespace csmacnz.Coveralls.Tests
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

        public Option<FileInfo[]> GetFiles(string directory)
        {
            //TODO
            throw new NotImplementedException();
        }

        public bool WriteFile(string outputFile, string fileData)
        {
            //todo: configure toggle
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