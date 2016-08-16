using System.Collections.Generic;
using System.Linq;
using csmacnz.Coveralls.DataAccess;

namespace csmacnz.Coveralls
{
    public class FileCoverageDataConverter
    {
        private readonly PathProcessor _pathProcessor;
        private readonly IFileSystem _fileSystem;

        public FileCoverageDataConverter(PathProcessor pathProcessor, IFileSystem fileSystem)
        {
            _pathProcessor = pathProcessor;
            _fileSystem = fileSystem;
        }

        public List<CoverageFile> Convert(List<FileCoverageData> coverageData, bool useRelativePaths)
        {
            return coverageData.Select(coverageFileData =>
            {
                var coverageBuilder = new CoverageFileBuilder(coverageFileData);

                var path = coverageFileData.FullPath;
                if (useRelativePaths)
                {
                    path = _pathProcessor.ConvertPath(coverageFileData.FullPath);
                }
                path = _pathProcessor.UnixifyPath(path);
                coverageBuilder.SetPath(path);

                var readAllText = _fileSystem.TryLoadFile(coverageFileData.FullPath);
                if (readAllText.HasValue)
                {
                    coverageBuilder.AddSource((string)readAllText);
                }

                return coverageBuilder.CreateFile();
            }).ToList();
        }
    }
}
