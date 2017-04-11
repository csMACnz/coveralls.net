using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Beefeater;
using csmacnz.Coveralls.Data;
using csmacnz.Coveralls.Parsers;
using csmacnz.Coveralls.Ports;

namespace csmacnz.Coveralls
{
    public class CoverageLoader
    {
        private readonly IFileSystem _fileSystem;

        public CoverageLoader(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public Result<List<CoverageFile>, LoadCoverageFilesError> LoadCoverageFiles(CoverageMode mode,
            PathProcessor pathProcessor, string modeInput, bool useRelativePaths)
        {
            List<CoverageFile> files;
            if (mode == CoverageMode.MonoCov)
            {
                var folderFiles = _fileSystem.GetFiles(modeInput);
                if (!folderFiles.HasValue)
                {
                    return LoadCoverageFilesError.InputFileNotFound;
                }
                var documents = LoadXDocuments(folderFiles);

                files = new MonoCoverParser(pathProcessor).GenerateSourceFiles(documents, useRelativePaths);
            }
            else if (mode == CoverageMode.Chutzpah)
            {
                if (!File.Exists(modeInput))
                {
                    return LoadCoverageFilesError.InputFileNotFound;
                }
                var source = _fileSystem.TryLoadFile(modeInput);
                if (source.HasValue)
                {
                    files = new ChutzpahJsonParser(pathProcessor).GenerateSourceFiles((string) source, useRelativePaths);
                }
                else
                {
                    return LoadCoverageFilesError.InputFileNotFound;
                }
            }
            else if (mode == CoverageMode.LCov)
            {
                var lines = _fileSystem.ReadAllLines(modeInput);

                if (lines.HasValue)
                {
                    return LoadCoverageFilesError.InputFileNotFound;
                }

                var coverageData = new LcovParser().GenerateSourceFiles((string[]) lines, useRelativePaths);

                files = BuildCoverageFiles(pathProcessor, useRelativePaths, coverageData);
            }
            else
            {
                //common xml-based single file formats
                var xmlParser = LoadXmlParser(mode);
                if (xmlParser == null)
                {
                    return LoadCoverageFilesError.ModeNotSupported;
                }

                var document = _fileSystem.TryLoadXDocumentFromFile(modeInput);

                if (!document.HasValue)
                {
                    return LoadCoverageFilesError.InputFileNotFound;
                }

                var coverageData = xmlParser.GenerateSourceFiles((XDocument) document);

                files = BuildCoverageFiles(pathProcessor, useRelativePaths, coverageData);
            }

            if (files == null)
            {
                return LoadCoverageFilesError.UnknownFilesMissingError;
            }
            return files;
        }

        private Dictionary<string, XDocument> LoadXDocuments(Option<FileInfo[]> folderFiles)
        {
            return ((FileInfo[]) folderFiles).Where(f => f.Name.EndsWith(".xml"))
                .ToDictionary(f => f.Name, f => (XDocument) _fileSystem.TryLoadXDocumentFromFile(f.FullName));
        }

        private List<CoverageFile> BuildCoverageFiles(PathProcessor pathProcessor, bool useRelativePaths,
            List<FileCoverageData> coverageData)
        {
            //This needs attention, since this is optional if source is on public github
            var files = coverageData.Select(coverageFileData =>
            {
                var coverageBuilder = new CoverageFileBuilder(coverageFileData);

                var path = coverageFileData.FullPath;
                if (useRelativePaths)
                {
                    path = pathProcessor.ConvertPath(coverageFileData.FullPath);
                }
                path = pathProcessor.UnixifyPath(path);
                coverageBuilder.SetPath(path);

                var readAllText = _fileSystem.TryLoadFile(coverageFileData.FullPath);
                if (readAllText.HasValue)
                {
                    coverageBuilder.AddSource((string) readAllText);
                }

                var coverageFile = coverageBuilder.CreateFile();
                return coverageFile;
            }).ToList();
            return files;
        }


        private IXmlCoverageParser LoadXmlParser(CoverageMode mode)
        {
            switch (mode)
            {
                case CoverageMode.OpenCover:
                    return new OpenCoverParser();
                case CoverageMode.DynamicCodeCoverage:
                    return new DynamicCodeCoverageParser();
                case CoverageMode.ExportCodeCoverage:
                    return new ExportCodeCoverageParser();
                default:
                    return null;
            }
        }
    }
}