using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using BCLExtensions;
using Beefeater;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace csmacnz.Coveralls
{
    public class Program
    {
        public static void Main(string[] argv)
        {
            var args = new MainArgs(argv, exit: true, version: (string)GetDisplayVersion());
            string repoToken;
            if (args.IsProvided("--repoToken"))
            {
                repoToken = args.OptRepotoken;
                if (repoToken.IsNullOrWhitespace())
                {
                    ExitWithError("parameter repoToken is required.");
                }
            }
            else
            {
                var variable = args.OptRepotokenvariable;
                if (variable.IsNullOrWhitespace())
                {
                    ExitWithError("parameter repoTokenVariable is required.");
                }

                repoToken = Environment.GetEnvironmentVariable(variable);
                if (repoToken.IsNullOrWhitespace())
                {
                    ExitWithError("No token found in Environment Variable '{0}'.".FormatWith(variable));
                }

            }
            var outputFile = args.IsProvided("--output") ? args.OptOutput : string.Empty;
            if (!string.IsNullOrWhiteSpace(outputFile) && File.Exists(outputFile))
            {
                Console.WriteLine("output file '{0}' already exists and will be overwritten.", outputFile);
            }

            var pathProcessor = new PathProcessor(args.IsProvided("--basePath") ? args.OptBasepath : null);

            List<CoverageFile> files;
            if (args.IsProvided("--multiple") && args.OptMultiple)
            {
                var modes = args.OptInput.Split(';');
                files = new List<CoverageFile>();
                foreach (var modekeyvalue in modes)
                {
                    var split= modekeyvalue.Split('=');
                    var rawMode = split[0];
                    var input = split[1];
                    var mode = GetMode(rawMode);
                    files.AddRange(LoadCoverageFiles(mode, pathProcessor, input, args.OptUserelativepaths));
                }
            }
            else
            {
                var mode = GetMode(args);
                files = LoadCoverageFiles(mode, pathProcessor, args.OptInput, args.OptUserelativepaths);
            }
            Debug.Assert(files != null);
            var gitData = ResolveGitData(args);

            var serviceName = ResolveServiceName(args);
            var serviceJobId = ResolveServiceJobId(args);
            var serviceNumber = ResolveServiceNumber(args);
            var pullRequestId = ResolvePullRequestId(args);
            var parallel = args.IsProvided("--parallel") && args.OptParallel;

            var data = new CoverallData
            {
                RepoToken = repoToken,
                ServiceJobId = serviceJobId.ValueOr("0"),
                ServiceName = serviceName.ValueOr("coveralls.net"),
                ServiceNumber = serviceNumber.ValueOr(null),
                PullRequestId = pullRequestId.ValueOr(null),
                SourceFiles = files.ToArray(),
                Parallel = parallel,
                Git = gitData.ValueOrDefault()
            };

            var fileData = JsonConvert.SerializeObject(data);
            if (!string.IsNullOrWhiteSpace(outputFile))
            {
                WriteFileData(fileData, outputFile);
            }
            if (!args.OptDryrun)
            {
                UploadCoverage(fileData, args.OptTreatuploaderrorsaswarnings);
            }
        }

        private static void UploadCoverage(string fileData, bool treatErrorsAsWarnings)
        {
            var uploadResult = new CoverallsService().Upload(fileData);
            if (!uploadResult.Successful)
            {
                var message = $"Failed to upload to coveralls\n{uploadResult.Error}";
                if (treatErrorsAsWarnings)
                {
                    Console.WriteLine(message);
                }
                else
                {
                    ExitWithError(message);
                }
            }
            else
            {
                Console.WriteLine("Coverage data uploaded to coveralls.");
            }
        }

        private static List<CoverageFile> LoadCoverageFiles(Option<CoverageMode> mode, PathProcessor pathProcessor, string inputArgument, bool useRelativePaths)
        {
            List<CoverageFile> files = null;
            if (!mode.HasValue)
            {
                ExitWithError("Unknown mode provided");
            }
            var coverageFiles = CoverageLoader.LoadCoverageFiles((CoverageMode) mode, pathProcessor, inputArgument, useRelativePaths);
            if (coverageFiles.Successful)
            {
                files = coverageFiles.Value;
            }
            else
            {
                switch (coverageFiles.Error)
                {
                    case LoadCoverageFilesError.InputFileNotFound:
                        ExitWithError($"Input file '{inputArgument}' cannot be found");
                        break;
                    case LoadCoverageFilesError.ModeNotSupported:
                        ExitWithError($"Could not process mode {mode}");
                        break;
                    case LoadCoverageFilesError.UnknownFilesMissingError:
                        ExitWithError($"Unknown Error Finding files processing mode {mode}");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return files;
        }

        private static Option<CoverageMode> GetMode(string mode)
        {
            if (string.Equals(mode, "monocov", StringComparison.OrdinalIgnoreCase))
            {
                return CoverageMode.MonoCov;
            }
            else if (string.Equals(mode, "chutzpah", StringComparison.OrdinalIgnoreCase))
            {
                return CoverageMode.Chutzpah;
            }
            else if (string.Equals(mode, "dynamiccodecoverage", StringComparison.OrdinalIgnoreCase))
            {
                return CoverageMode.DynamicCodeCoverage;
            }
            else if (string.Equals(mode, "exportcodecoverage", StringComparison.OrdinalIgnoreCase))
            {
                return CoverageMode.ExportCodeCoverage;
            }
            else if (string.Equals(mode, "opencover", StringComparison.OrdinalIgnoreCase))
            {
                return CoverageMode.OpenCover;
            }
            else if (string.Equals(mode, "lcov", StringComparison.OrdinalIgnoreCase))
            {
                return CoverageMode.LCov;
            }
            else
            {
                return Option<CoverageMode>.None;
            }
        }

        private static Option<CoverageMode> GetMode(MainArgs args)
        {
            if (args.IsProvided("--monocov") && args.OptMonocov)
            {
                return CoverageMode.MonoCov;
            }
            else if (args.IsProvided("--chutzpah") && args.OptChutzpah)
            {
                return CoverageMode.Chutzpah;
            }
            else if (args.IsProvided("--dynamiccodecoverage") && args.OptDynamiccodecoverage)
            {
                return CoverageMode.DynamicCodeCoverage;
            }
            else if (args.IsProvided("--exportcodecoverage") && args.OptExportcodecoverage)
            {
                return CoverageMode.ExportCodeCoverage;
            }
            else if (args.IsProvided("--opencover") && args.OptOpencover)
            {
                return CoverageMode.OpenCover;
            }
            else if (args.IsProvided("--lcov") && args.OptLcov)
            {
                return CoverageMode.LCov;
            }
            
            return Option<CoverageMode>.None; //Unreachable
        }

        private static NotNull<string> GetDisplayVersion()
        {
            return FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location).ProductVersion;
        }
        
        [ContractAnnotation("=>halt")]
        private static void ExitWithError(string message)
        {
            Console.Error.WriteLine(message);
            Environment.Exit(1);
        }

        private static Option<string> ResolveServiceName(MainArgs args)
        {
            if (args.IsProvided("--serviceName")) return args.OptServicename;
            var isAppVeyor = new EnvironmentVariables().GetEnvironmentVariable("APPVEYOR");
            if (isAppVeyor == "True") return "appveyor";
            return null;
        }

        private static Option<string> ResolveServiceJobId(MainArgs args)
        {
            if (args.IsProvided("--jobId")) return args.OptJobid;
            var jobId = new EnvironmentVariables().GetEnvironmentVariable("APPVEYOR_JOB_ID");
            if (jobId.IsNotNullOrWhitespace()) return jobId;
            return null;
        }

        private static Option<string> ResolveServiceNumber(MainArgs args)
        {
            if (args.IsProvided("--serviceNumber")) return args.OptServicenumber;
            var jobId = new EnvironmentVariables().GetEnvironmentVariable("APPVEYOR_BUILD_NUMBER");
            if (jobId.IsNotNullOrWhitespace()) return jobId;
            return null;
        }

        private static Option<string> ResolvePullRequestId(MainArgs args)
        {
            if (args.IsProvided("--pullRequest")) return args.OptPullrequest;
            var prId = new EnvironmentVariables().GetEnvironmentVariable("APPVEYOR_PULL_REQUEST_NUMBER");
            if (prId.IsNotNullOrWhitespace()) return prId;
            return null;
        }

        private static Option<GitData> ResolveGitData(MainArgs args)
        {
            var providers = new List<IGitDataResolver>
            {
                new CommandLineGitDataResolver(args), new AppVeyorGitDataResolver(new EnvironmentVariables())
            };

            return providers.Where(p => p.CanProvideData()).Select(p => p.GenerateData()).FirstOrDefault();
        }

        private static void WriteFileData(string fileData, string outputFile)
        {
            if (!new FileSystem().WriteFile(outputFile, fileData))
            {
                Console.WriteLine("Failed to write data to output file '{0}'.", outputFile);
            }
        }
    }
}
