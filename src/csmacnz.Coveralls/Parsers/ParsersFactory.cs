using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            throw new InvalidDataException("Missing format parser!");
        }

    }
}
