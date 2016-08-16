using System;
using System.Collections.Generic;
using System.Linq;
using csmacnz.Coveralls.DataAccess;
using DocoptNet;

namespace csmacnz.Coveralls.Parsers
{
    public class FolderParser : IParser
    {
        private IFileSystem _fileSystem;
        private PathProcessor _pathProcessor;

        public FolderParser(PathProcessor pathProcessor, IFileSystem fileSystem)
        {
            _pathProcessor = pathProcessor;
            _fileSystem = fileSystem;
        }

        public List<CoverageFile> GenerateSourceFiles(string path, bool useRelativePaths)
        {
            var factory = new ParsersFactory(_pathProcessor, _fileSystem);
            string[] items = _fileSystem.GetFileSystemEntries(path);
            List<CoverageFile> files = new List<CoverageFile>();
            foreach (var item in items)
            {
                var parser = factory.Create(item);
                if (parser == null)
                {
                    continue;
                }
                files.AddRange(parser.GenerateSourceFiles(item, useRelativePaths));
            }
            return files;
        }
    }
}
