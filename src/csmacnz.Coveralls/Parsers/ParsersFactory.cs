using System.IO;
using csmacnz.Coveralls.DataAccess;

namespace csmacnz.Coveralls.Parsers
{
    public class ParsersFactory
    {
        private readonly PathProcessor _pathProcessor;
        private readonly IFileSystem _fileSystem;

        public ParsersFactory(PathProcessor pathProcessor, IFileSystem fileSystem)
        {
            _pathProcessor = pathProcessor;
            _fileSystem = fileSystem;
        }

        public IParser Create(MainArgs args)
        {
            if (args.IsProvided("--monocov") && args.OptMonocov)
            {
                return new MonoCoverParser(_pathProcessor, _fileSystem);
            }
            if (args.IsProvided("--chutzpah") && args.OptChutzpah)
            {
                return new ChutzpahJsonParser(_pathProcessor, _fileSystem);
            }
            if (args.IsProvided("--dynamiccodecoverage") && args.OptDynamiccodecoverage)
            {
                return new DynamicCodeCoverageParser(_pathProcessor, _fileSystem);
            }
            if (args.IsProvided("--exportcodecoverage") && args.OptExportcodecoverage)
            {
                return new ExportCodeCoverageParser(_pathProcessor, _fileSystem);
            }
            if (args.IsProvided("--opencover") && args.OptOpencover)
            {
                return new OpenCoverParser(_pathProcessor, _fileSystem);
            }
            if (args.IsProvided("--folder") && args.OptFolder)
            {
                return new FolderParser(_pathProcessor, _fileSystem);
            }
            throw new InvalidDataException("Missing format parser!");
        }

        public IParser Create(string path)
        {
            if (path.EndsWith("monocov") && _fileSystem.IsDirectory(path))
            {
                return new MonoCoverParser(_pathProcessor, _fileSystem);
            }
            if (path.EndsWith("chutzpah") && _fileSystem.IsFile(path))
            {
                return new ChutzpahJsonParser(_pathProcessor, _fileSystem);
            }
            if (path.EndsWith("dynamiccodecoverage") && _fileSystem.IsFile(path))
            {
                return new DynamicCodeCoverageParser(_pathProcessor, _fileSystem);
            }
            if (path.EndsWith("exportcodecoverage") && _fileSystem.IsFile(path))
            {
                return new ExportCodeCoverageParser(_pathProcessor, _fileSystem);
            }
            if (path.EndsWith("opencover") && _fileSystem.IsFile(path))
            {
                return new OpenCoverParser(_pathProcessor, _fileSystem);
            }
            return null;
        }
    }
}
