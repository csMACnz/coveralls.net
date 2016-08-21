using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Beefeater;

namespace csmacnz.Coveralls
{
    public class CoverageLoader
    {
        private readonly IFileSystem _fileSystem;

        public CoverageLoader(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public Result<List<CoverageFile>, LoadCoverageFilesError> LoadCoverageFiles(CoverageMode mode, PathProcessor pathProcessor, string modeInput, bool useRelativePaths)
        {
            List<CoverageFile> files;
            if (mode == CoverageMode.MonoCov)
            {
                var folderFiles = _fileSystem.GetFiles(modeInput);
                if (!folderFiles.HasValue)
                {
                    return LoadCoverageFilesError.InputFileNotFound;
                }
                Dictionary<string, XDocument> documents = ((FileInfo[])folderFiles).Where(f => f.Name.EndsWith(".xml")).ToDictionary(f => f.Name, f => (XDocument)_fileSystem.TryLoadXDocumentFromFile(f.FullName));

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
            else
            {
                List<FileCoverageData> coverageData;
                if (mode == CoverageMode.DynamicCodeCoverage)
                {
                    var document = _fileSystem.TryLoadXDocumentFromFile(modeInput);

                    if (!document.HasValue)
                    {
                        return LoadCoverageFilesError.InputFileNotFound;
                    }

                    coverageData = new DynamicCodeCoverageParser().GenerateSourceFiles((XDocument)document);
                }
                else if (mode == CoverageMode.ExportCodeCoverage)
                {
                    var document = _fileSystem.TryLoadXDocumentFromFile(modeInput);

                    if (!document.HasValue)
                    {
                        return LoadCoverageFilesError.InputFileNotFound;
                    }

                    coverageData = new ExportCodeCoverageParser().GenerateSourceFiles((XDocument)document);
                }
                else if (mode == CoverageMode.OpenCover)
                {
                    var document = _fileSystem.TryLoadXDocumentFromFile(modeInput);

                    if (!document.HasValue)
                    {
                        return LoadCoverageFilesError.InputFileNotFound;
                    }

                    coverageData = new OpenCoverParser().GenerateSourceFiles((XDocument)document);
                }
                else if (mode == CoverageMode.LCov)
                {
                    var lines = _fileSystem.ReadAllLines(modeInput);

                    if (lines.HasValue)
                    {
                        return LoadCoverageFilesError.InputFileNotFound;
                    }
                    
                    coverageData = new LcovParser().GenerateSourceFiles((string[])lines, useRelativePaths);
                }
                else
                {
                    return LoadCoverageFilesError.ModeNotSupported;
                }

                //This needs attention, since this is optional if source is on public github
                files = coverageData.Select(coverageFileData =>
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
            }
            if (files == null)
            {
                return LoadCoverageFilesError.UnknownFilesMissingError;
            }
            return files;
        }
    }
}