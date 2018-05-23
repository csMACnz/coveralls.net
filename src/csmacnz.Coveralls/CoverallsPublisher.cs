using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using BCLExtensions;
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
        private readonly IFileSystem _fileSystem;
        private readonly ICoverallsService _coverallsService;

        public CoverallsPublisher(
            IConsole console,
            IFileSystem fileSystem,
            ICoverallsService coverallsService)
        {
            _console = console;
            _fileSystem = fileSystem;
            _coverallsService = coverallsService;
        }

        public Result<Unit, string> Run(
            ConfigurationSettings settings,
            Either<GitData, CommitSha> gitData,
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
                ServiceNumber = metadata.ServiceBuildNumber,
                PullRequestId = metadata.PullRequestId,
                SourceFiles = files.Value.ToArray(),
                Parallel = metadata.Parallel,
            };

            gitData.Match(
                git =>
                {
                    data.Git = git;
                },
                sha =>
                {
                    data.CommitSha = sha.Value;
                });

            var fileData = JsonConvert.SerializeObject(data);
            if (!string.IsNullOrWhiteSpace(outputFile))
            {
                WriteFileData(_fileSystem, fileData, outputFile);
            }

            if (!settings.DryRun)
            {
                var uploadResult = UploadCoverage(fileData);
                if (!uploadResult.Successful)
                {
                    if (settings.TreatUploadErrorsAsWarnings)
                    {
                        _console.WriteLine(uploadResult.Error);
                    }
                    else
                    {
                        return uploadResult.Error;
                    }
                }
            }

            return Unit.Default;
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

        private Result<List<CoverageFile>, string> BuildCoverageFiles(ConfigurationSettings args)
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

            Debug.Assert(files != null, "Files should always be returned.");
            return files;
        }

        private Result<Unit, string> UploadCoverage(string fileData)
        {
            var uploadResult = _coverallsService.Upload(fileData);
            if (!uploadResult.Successful)
            {
                var message = $"Failed to upload to coveralls\n{uploadResult.Error}";
                return message;
            }

            _console.WriteLine("Coverage data uploaded to coveralls.");
            return Unit.Default;
        }

        private Result<List<CoverageFile>, string> LoadCoverageFiles(
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
                        throw new NotSupportedException($"Unknown value '{coverageFiles.Error}' returned from 'LoadCoverageFiles'.");
                }
            }

            return coverageFiles.Value;
        }
    }
}
