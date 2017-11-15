using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Beefeater;
using csmacnz.Coveralls.Adapters;
using csmacnz.Coveralls.Data;
using csmacnz.Coveralls.Ports;
using Newtonsoft.Json;

namespace csmacnz.Coveralls
{
    public class CoverallsPublisher
    {
        private readonly IConsole _console;
        private static IFileSystem _fileSystem;

        public CoverallsPublisher(IConsole console, IFileSystem fileSystem)
        {
            _console = console;
            _fileSystem = fileSystem;
        }

        public Result<SuccessResult, string> Run(
            ConfigurationSettings settings,
            GitData gitData,
            CoverageMetadata metadata)
        {
            var outputFile = ResolveOutpuFile(settings);

            // Main Processing
            var files = BuildCoverageFiles(settings);
            if (!files.Successful)
            {
                return files.Error;
            }

            var data = new CoverallData
            {
                RepoToken = settings.RepoToken,
                ServiceJobId = metadata.ServiceJobId,
                ServiceName = metadata.ServiceName,
                ServiceNumber = metadata.ServiceNumber,
                PullRequestId = metadata.PullRequestId,
                SourceFiles = files.Value.ToArray(),
                Parallel = metadata.Parallel,
                Git = gitData
            };

            var fileData = JsonConvert.SerializeObject(data);
            if (!string.IsNullOrWhiteSpace(outputFile))
            {
                WriteFileData(_fileSystem, fileData, outputFile);
            }

            if (!settings.DryRun)
            {
                var uploadResult = UploadCoverage(fileData);
                if (settings.TreatUploadErrorsAsWarnings)
                {
                    _console.WriteLine(uploadResult.Error);
                }
                else
                {
                    return uploadResult.Error;
                }
            }

            return SuccessResult.Value;
        }

        private string ResolveOutpuFile(ConfigurationSettings settings)
        {
            var outputFile = settings.OutputFile;
            if (!string.IsNullOrWhiteSpace(outputFile) && File.Exists(outputFile))
            {
                _console.WriteLine($"output file '{outputFile}' already exists and will be overwritten.");
            }

            return outputFile;
        }

        private void WriteFileData(IFileStorer fileStorer, string fileData, string outputFile)
        {
            if (!fileStorer.WriteFile(outputFile, fileData))
            {
                _console.WriteLine($"Failed to write data to output file '{outputFile}'.");
            }
        }

        private static Result<List<CoverageFile>, string> BuildCoverageFiles(ConfigurationSettings args)
        {
            var pathProcessor = new PathProcessor(args.BasePath);

            var files = new List<CoverageFile>();
            foreach (var source in args.CoverageSources)
            {
                var mode = source.Mode;
                var coverageFiles = LoadCoverageFiles(mode, pathProcessor, source.Input, args.UseRelativePaths);
                if (!coverageFiles.Successful)
                {
                    return coverageFiles.Error;
                }

                files.AddRange(coverageFiles.Value);
            }

            Debug.Assert(files != null);
            return files;
        }

        private Result<SuccessResult, string> UploadCoverage(string fileData)
        {
            var uploadResult = new CoverallsService().Upload(fileData);
            if (!uploadResult.Successful)
            {
                var message = $"Failed to upload to coveralls\n{uploadResult.Error}";
                return message;
            }

            _console.WriteLine("Coverage data uploaded to coveralls.");
            return SuccessResult.Value;
        }

        private static Result<List<CoverageFile>, string> LoadCoverageFiles(
            CoverageMode mode,
            PathProcessor pathProcessor,
            string inputArgument,
            bool useRelativePaths)
        {
            var loader = new CoverageLoader(_fileSystem);
            var coverageFiles = loader.LoadCoverageFiles(
                mode,
                pathProcessor,
                inputArgument,
                useRelativePaths);

            if (!coverageFiles.Successful)
            {
                switch (coverageFiles.Error)
                {
                    case LoadCoverageFilesError.InputFileNotFound:
                        return $"Input file '{inputArgument}' cannot be found";
                    case LoadCoverageFilesError.ModeNotSupported:
                        return $"Could not process mode {mode}";
                    case LoadCoverageFilesError.UnknownFilesMissingError:
                        return $"Unknown Error Finding files processing mode {mode}";
                    default:
                        throw new ArgumentOutOfRangeException(nameof(coverageFiles.Error));
                }
            }

            return coverageFiles.Value;
        }
    }
}
