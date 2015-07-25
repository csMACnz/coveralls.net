using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
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

            List<CoverageFile> files;
            if (args.IsProvided("--monocov") && args.OptMonocov)
            {
                var fileName = args.OptInput;
                if (!Directory.Exists(fileName))
                {
                    ExitWithError("Input file '" + fileName + "' cannot be found");
                }
                Dictionary<string, XDocument> documents =
                    new DirectoryInfo(fileName).GetFiles()
                        .Where(f => f.Name.EndsWith(".xml"))
                        .ToDictionary(f => f.Name, f => XDocument.Load(f.FullName));

                files = new MonoCoverParser(pathProcessor).GenerateSourceFiles(documents, args.OptUserelativepaths);
            }
            else
            {
                List<FileCoverageData> coverageData;
                if (args.IsProvided("--dynamiccodecoverage") && args.OptDynamiccodecoverage)
                {
                    var fileName = args.OptInput;
                    if (!File.Exists(fileName))
                    {
                        ExitWithError("Input file '" + fileName + "' cannot be found");
                    }

                    var document = XDocument.Load(fileName);

                    coverageData = new DynamicCodeCoverageParser().GenerateSourceFiles(document);
                }
                else
                {
                    var fileName = args.OptInput;
                    if (!File.Exists(fileName))
                    {
                        ExitWithError("Input file '" + fileName + "' cannot be found");
                    }

                    var document = XDocument.Load(fileName);

                    coverageData = new OpenCoverParser().GenerateSourceFiles(document);
                }

                files = coverageData.Select(coverageFileData =>
                {
                    var coverageBuilder = new CoverageFileBuilder(coverageFileData);

                    var path = coverageFileData.FullPath;
                    if (args.OptUserelativepaths)
                    {
                        path = pathProcessor.ConvertPath(coverageFileData.FullPath);
                    }
                    path = pathProcessor.UnixifyPath(path);
                    coverageBuilder.SetPath(path);

                    var readAllText = new FileSystem().TryLoadFile(coverageFileData.FullPath);
                    if (readAllText.HasValue)
                    {
                        coverageBuilder.AddSource((string)readAllText);
                    }

                    var coverageFile = coverageBuilder.CreateFile();
                    return coverageFile;
                }).ToList();
            }

            var gitData = ResolveGitData(args);

            var serviceJobId = ResolveServiceJobId(args);

            string serviceName = args.IsProvided("--serviceName") ? args.OptServicename : "coveralls.net";
            var data = new CoverallData
            {
                RepoToken = repoToken,
                ServiceJobId = serviceJobId.ValueOr("0"),
                ServiceName = serviceName,
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
                var uploadResult = Upload(@"https://coveralls.io/api/v1/jobs", fileData);
                if (!uploadResult.Successful)
                {
                    ExitWithError(string.Format("Failed to upload to coveralls\n{0}", uploadResult.Error));
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

        private static Option<string> ResolveServiceJobId(MainArgs args)
        {
            if (args.IsProvided("--jobId")) return args.OptJobid;
            var jobId = new EnvironmentVariables().GetEnvironmentVariable("APPVEYOR_JOB_ID");
            if (jobId.IsNotNullOrWhitespace()) return jobId;
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

        //TODO(csMACnz): change from bool to Unit or a simplified Result<TError> as a thing?
        private static Result<bool, string> Upload(string url, string fileData)
        {
            using (HttpContent stringContent = new StringContent(fileData))
            {
                using (var client = new HttpClient())
                using (var formData = new MultipartFormDataContent())
                {
                    formData.Add(stringContent, "json_file", "coverage.json");

                    var response = client.PostAsync(url, formData).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        var message = TryGetJsonMessageFromResponse(content).ValueOr(content);

                        return string.Format("{0} - {1}", response.StatusCode, message);
                    }
                    return true;
                }
            }
        }

        private static Option<string> TryGetJsonMessageFromResponse(string content)
        {
            try
            {
                dynamic result = JsonConvert.DeserializeObject(content);
                return result.message;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
