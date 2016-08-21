using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Beefeater;

namespace csmacnz.Coveralls
{
    public class CoverageLoader
    {
        public static Result<List<CoverageFile>, LoadCoverageFilesError> LoadCoverageFiles(CoverageMode mode, PathProcessor pathProcessor, string modeInput, bool useRelativePaths)
        {
            List<CoverageFile> files;
            if (mode == CoverageMode.MonoCov)
            {
                if (!Directory.Exists(modeInput))
                {
                    return LoadCoverageFilesError.InputFileNotFound;
                }
                Dictionary<string, XDocument> documents = new DirectoryInfo(modeInput).GetFiles().Where(f => f.Name.EndsWith(".xml")).ToDictionary(f => f.Name, f => XDocument.Load(f.FullName));

                files = new MonoCoverParser(pathProcessor).GenerateSourceFiles(documents, useRelativePaths);
            }
            else if (mode == CoverageMode.Chutzpah)
            {
                if (!File.Exists(modeInput))
                {
                    return LoadCoverageFilesError.InputFileNotFound;
                }
                var source = new FileSystem().TryLoadFile(modeInput);
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
                    if (!File.Exists(modeInput))
                    {
                        return LoadCoverageFilesError.InputFileNotFound;
                    }

                    var document = XDocument.Load(modeInput);

                    coverageData = new DynamicCodeCoverageParser().GenerateSourceFiles(document);
                }
                else if (mode == CoverageMode.ExportCodeCoverage)
                {
                    if (!File.Exists(modeInput))
                    {
                        return LoadCoverageFilesError.InputFileNotFound;
                    }

                    var document = XDocument.Load(modeInput);

                    coverageData = new ExportCodeCoverageParser().GenerateSourceFiles(document);
                }
                else if (mode == CoverageMode.OpenCover)
                {
                    if (!File.Exists(modeInput))
                    {
                        return LoadCoverageFilesError.InputFileNotFound;
                    }

                    var document = XDocument.Load(modeInput);

                    coverageData = new OpenCoverParser().GenerateSourceFiles(document);
                }
                else if (mode == CoverageMode.LCov)
                {
                    if (!File.Exists(modeInput))
                    {
                        return LoadCoverageFilesError.InputFileNotFound;
                    }
                    var lines = File.ReadAllLines(modeInput);

                    coverageData = new LcovParser().GenerateSourceFiles(lines, useRelativePaths);
                }
                else
                {
                    return LoadCoverageFilesError.ModeNotSupported;
                }

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

                    var readAllText = new FileSystem().TryLoadFile(coverageFileData.FullPath);
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