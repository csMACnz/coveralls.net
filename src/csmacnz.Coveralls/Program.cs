using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace csmacnz.Coveralls
{
    public class Program
    {
        public static void Main(string[] argv)
        {
            var args = new MainArgs(argv, exit: true, version: Assembly.GetEntryAssembly().GetName().Version);
            var repoToken = args.OptRepotoken;
            if (string.IsNullOrWhiteSpace(repoToken))
            {
                Console.Error.WriteLine("parameter repoToken is required.");
                Console.WriteLine(MainArgs.Usage);
                Environment.Exit(1);
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
                    Console.Error.WriteLine("Input file '" + fileName + "' cannot be found");
                    Environment.Exit(1);
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
                        Console.Error.WriteLine("Input file '" + fileName + "' cannot be found");
                        Environment.Exit(1);
                    }

                    var document = XDocument.Load(fileName);

                    coverageData = new DynamicCodeCoverageParser().GenerateSourceFiles(document);
                }
                else
                {
                    var fileName = args.OptInput;
                    if (!File.Exists(fileName))
                    {
                        Console.Error.WriteLine("Input file '" + fileName + "' cannot be found");
                        Environment.Exit(1);
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
                    if (readAllText != null)
                    {
                        coverageBuilder.AddSource(readAllText);
                    }

                    var coverageFile = coverageBuilder.CreateFile();
                    return coverageFile;
                }).ToList();
            }

            var gitData = ResolveGitData(args);

            var serviceJobId = args.IsProvided("--jobId") ? args.OptJobid : "0";

            string serviceName = args.IsProvided("--serviceName") ? args.OptServicename : "coveralls.net";
            var data = new CoverallData
            {
                RepoToken = repoToken,
                ServiceJobId = serviceJobId,
                ServiceName = serviceName,
                SourceFiles = files.ToArray(),
                Git = gitData
            };

            var fileData = JsonConvert.SerializeObject(data);
            if (!string.IsNullOrWhiteSpace(outputFile))
            {
                WriteFileData(fileData, outputFile);
            }
            if (!args.OptDryrun)
            {
                var uploaded = Upload(@"https://coveralls.io/api/v1/jobs", fileData);
                if (!uploaded)
                {
                    Console.Error.WriteLine("Failed to upload to coveralls");
                    Environment.Exit(1);
                }
            }
        }

        private static GitData ResolveGitData(MainArgs args)
        {
            var providers = new List<IGitDataResolver>
            {
                new AppVeyorGitDataResolver(),
                new CommandLineGitDataResolver(args),
                
            };
            return (from provider in providers where provider.CanProvideData() select provider.GenerateData()).FirstOrDefault();
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

        private static bool Upload(string url, string fileData)
        {
            HttpContent stringContent = new StringContent(fileData);

            using (var client = new HttpClient())
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(stringContent, "json_file", "coverage.json");

                var response = client.PostAsync(url, formData).Result;

                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
