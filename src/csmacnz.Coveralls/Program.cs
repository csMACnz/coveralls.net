using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using BCLExtensions;
using Beefeater;
using csmacnz.Coveralls.DataAccess;
using csmacnz.Coveralls.Parsers;
using Newtonsoft.Json;

namespace csmacnz.Coveralls
{
    public class Program
    {
        public static void Main(string[] argv)
        {
            var args = new MainArgs(argv, exit: true, version: (string)GetDisplayVersion());

            ValidateInput(args);

            var outputFile = args.IsProvided("--output") ? args.OptOutput : string.Empty;
            if (!string.IsNullOrWhiteSpace(outputFile) && File.Exists(outputFile))
            {
                Console.WriteLine("output file '{0}' already exists and will be overwritten.", outputFile);
            }

            var pathProcessor = new PathProcessor(args.IsProvided("--basePath") ? args.OptBasepath : null);
            var parser = new ParsersFactory(pathProcessor, new FileSystem()).Create(args);
            List<CoverageFile> files = parser.GenerateSourceFiles(args.OptInput, args.OptUserelativepaths);

            var repoToken = ResolveRepositoryToken(args);
            var gitData = ResolveGitData(args);
            var serviceJobId = ResolveServiceJobId(args);
            var pullRequestId = ResolvePullRequestId(args);
            var serviceName = args.IsProvided("--serviceName") ? args.OptServicename : "coveralls.net";

            var data = new CoverallData
            {
                RepoToken = repoToken,
                ServiceJobId = serviceJobId.ValueOr("0"),
                ServiceName = serviceName,
                PullRequestId = pullRequestId.ValueOr(null),
                SourceFiles = files.ToArray(),
                Git = gitData.ValueOrDefault()
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
                    var message = string.Format("Failed to upload to coveralls\n{0}", uploadResult.Error);
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

        private static NotNull<string> GetDisplayVersion()
        {
            return FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location).ProductVersion;
        }

        private static void ExitWithError(string message)
        {
            Console.Error.WriteLine(message);
            Environment.Exit(1);
        }

        private static void ValidateInput(MainArgs args)
        {
            if (args.IsProvided("--monocov") && args.OptMonocov)
            {
                var fileName = args.OptInput;
                if (!Directory.Exists(fileName))
                {
                    ExitWithError("Input directory '" + fileName + "' cannot be found");
                }
            }
            else if (args.IsProvided("--chutzpah") && args.OptChutzpah ||
                     args.IsProvided("--dynamiccodecoverage") && args.OptDynamiccodecoverage ||
                     args.IsProvided("--exportcodecoverage") && args.OptExportcodecoverage ||
                     args.IsProvided("--opencover") && args.OptOpencover)
            {
                var fileName = args.OptInput;
                if (!File.Exists(fileName))
                {
                    ExitWithError("Input file '" + fileName + "' cannot be found");
                }
            }
        }

        private static string ResolveRepositoryToken(MainArgs args)
        {
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
            return repoToken;
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
                new CommandLineGitDataResolver(args),
                new AppVeyorGitDataResolver(new EnvironmentVariables())
            };

            return providers.Where(p=>p.CanProvideData()).Select(p=>p.GenerateData()).FirstOrDefault();
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
