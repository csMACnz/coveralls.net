using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using BCLExtensions;
using Beefeater;
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

            List<CoverageFile> files = null;
            if (args.IsProvided("--multiple") && args.OptMultiple)
            {
                var modes = args.OptInput.Split(';');
                files = new List<CoverageFile>();
                foreach (var modekeyvalue in modes)
                {
                    var split= modekeyvalue.Split('=');
                    var mode = split[0];
                    var input = split[1];
                    var coverageFiles = LoadCoverageFiles(mode, pathProcessor, input, args.OptUserelativepaths);
                    if (coverageFiles.Successful)
                    {
                        files.AddRange(coverageFiles.Value);
                    }
                    else
                    {
                        switch (coverageFiles.Error)
                        {
                            case LoadCoverageFilesError.UnknownModeProvided:
                                ExitWithError($"Unknown mode provided with '--multiple': {modekeyvalue}");
                                break;
                            case LoadCoverageFilesError.InputFileNotFound:
                                ExitWithError("Input file '" + args.OptInput + "' cannot be found");
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
            else
            {
                var mode = GetMode(args);

                var coverageFiles = LoadCoverageFiles(mode, pathProcessor, args.OptInput, args.OptUserelativepaths);
                if (coverageFiles.Successful)
                {
                    files = coverageFiles.Value;
                }
                else
                {
                    switch (coverageFiles.Error)
                    {
                        case LoadCoverageFilesError.UnknownModeProvided:
                            ExitWithError($"Unknown mode provided: {mode}");
                            break;
                        case LoadCoverageFilesError.InputFileNotFound:
                            ExitWithError("Input file '" + args.OptInput + "' cannot be found");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            Debug.Assert(files != null);
            var gitData = ResolveGitData(args);

            var serviceJobId = ResolveServiceJobId(args);
            var pullRequestId = ResolvePullRequestId(args);

            string serviceName = args.IsProvided("--serviceName") ? args.OptServicename : "coveralls.net";
            var data = new CoverallData
            {
                RepoToken = repoToken, ServiceJobId = serviceJobId.ValueOr("0"), ServiceName = serviceName, PullRequestId = pullRequestId.ValueOr(null), SourceFiles = files.ToArray(), Git = gitData.ValueOrDefault()
            };

            var fileData = JsonConvert.SerializeObject(data);
            if (!string.IsNullOrWhiteSpace(outputFile))
            {
                WriteFileData(fileData, outputFile);
            }
            if (!args.OptDryrun)
            {
                var uploadResult = new CoverallsService().Upload(fileData);
                if (!uploadResult.Successful)
                {
                    var message = $"Failed to upload to coveralls\n{uploadResult.Error}";
                    if (args.OptTreatuploaderrorsaswarnings)
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
        }

        private static Result<List<CoverageFile>, LoadCoverageFilesError> LoadCoverageFiles(string mode, PathProcessor pathProcessor, string modeInput, bool useRelativePaths)
        {
            List<CoverageFile> files;
            if (string.Equals(mode, "monocov", StringComparison.OrdinalIgnoreCase))
            {
                if (!Directory.Exists(modeInput))
                {
                    ExitWithError("Input file '" + modeInput + "' cannot be found");
                }
                Dictionary<string, XDocument> documents = new DirectoryInfo(modeInput).GetFiles().Where(f => f.Name.EndsWith(".xml")).ToDictionary(f => f.Name, f => XDocument.Load(f.FullName));

                files = new MonoCoverParser(pathProcessor).GenerateSourceFiles(documents, useRelativePaths);
            }
            else if (string.Equals(mode, "chutzpah", StringComparison.OrdinalIgnoreCase))
            {
                if (!File.Exists(modeInput))
                {
                    return LoadCoverageFilesError.InputFileNotFound;
                }
                var source = File.ReadAllText(modeInput);
                files = new ChutzpahJsonParser(pathProcessor).GenerateSourceFiles(source, useRelativePaths);
            }
            else
            {
                List<FileCoverageData> coverageData;
                if (string.Equals(mode, "dynamiccodecoverage", StringComparison.OrdinalIgnoreCase))
                {
                    if (!File.Exists(modeInput))
                    {
                        return LoadCoverageFilesError.InputFileNotFound;
                    }

                    var document = XDocument.Load(modeInput);

                    coverageData = new DynamicCodeCoverageParser().GenerateSourceFiles(document);
                }
                else if (string.Equals(mode, "exportcodecoverage", StringComparison.OrdinalIgnoreCase))
                {
                    if (!File.Exists(modeInput))
                    {
                        return LoadCoverageFilesError.InputFileNotFound;
                    }

                    var document = XDocument.Load(modeInput);

                    coverageData = new ExportCodeCoverageParser().GenerateSourceFiles(document);
                }
                else if (string.Equals(mode, "opencover", StringComparison.OrdinalIgnoreCase))
                {
                    if (!File.Exists(modeInput))
                    {
                        return LoadCoverageFilesError.InputFileNotFound;
                    }

                    var document = XDocument.Load(modeInput);

                    coverageData = new OpenCoverParser().GenerateSourceFiles(document);
                }
                else
                {
                    return LoadCoverageFilesError.UnknownModeProvided;
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
            return files;
        }

        private enum LoadCoverageFilesError
        {
            UnknownModeProvided,
            InputFileNotFound
        }

        private static string GetMode(MainArgs args)
        {
            if (args.IsProvided("--monocov") && args.OptMonocov)
            {
                return "monocov";
            }
            else if (args.IsProvided("--chutzpah") && args.OptChutzpah)
            {
                return "chutzpah";
            }
            else if (args.IsProvided("--dynamiccodecoverage") && args.OptDynamiccodecoverage)
            {
                return "dynamiccodecoverage";
            }
            else if (args.IsProvided("--exportcodecoverage") && args.OptExportcodecoverage)
            {
                return "exportcodecoverage";
            }
            else if (args.IsProvided("--opencover") && args.OptOpencover)
            {
                return "opencover";
            }
            ExitWithError("Unknown file process mode");
            return null; //Unreachable
        }

        private static NotNull<string> GetDisplayVersion()
        {
            return FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location).ProductVersion;
        }

        private static void ExitWithError(string message)
        {
            Console.Error.WriteLine(message);
            Environment.Exit(1);
        }

        private static Option<string> ResolveServiceJobId(MainArgs args)
        {
            if (args.IsProvided("--jobId")) return args.OptJobid;
            var jobId = new EnvironmentVariables().GetEnvironmentVariable("APPVEYOR_JOB_ID");
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
            try
            {
                File.WriteAllText(outputFile, fileData);
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to write data to output file '{0}'.", outputFile);
            }
        }
    }
}
