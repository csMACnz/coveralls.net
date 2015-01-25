using System;
using System.Collections.Generic;
using System.IO;
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
            var fileName = args.OptInput;
            if (!File.Exists(fileName))
            {
                Console.Error.WriteLine("Input file '" + fileName + "' cannot be found");
                Environment.Exit(1);
            }
            var document = XDocument.Load(fileName);

            List<CoverageFile> files = new OpenCoverParser(new FileSystem()).GenerateSourceFiles(document);

            GitData gitData = null;
            var commitId = args.OptCommitid ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(commitId))
            {
                var committerName = args.OptCommitauthor ?? string.Empty;
                var comitterEmail = args.OptCommitemail ?? string.Empty;
                var commitMessage = args.OptCommitmessage ?? string.Empty;
                gitData = new GitData
                {
                    Head = new GitHead
                    {
                        Id = commitId,
                        AuthorName = committerName,
                        AuthorEmail = comitterEmail,
                        CommitterName = committerName,
                        ComitterEmail = comitterEmail,
                        Message = commitMessage
                    },
                    Branch = args.OptCommitbranch ?? string.Empty
                };
            }

            var serviceJobId = args.OptJobid ?? "0";

            var data = new CoverallData
            {
                RepoToken = repoToken,
                ServiceJobId = serviceJobId,
                ServiceName = "coveralls.net",
                SourceFiles = files.ToArray(),
                Git = gitData
            };

            var fileData = JsonConvert.SerializeObject(data);
            var uploaded = Upload(@"https://coveralls.io/api/v1/jobs", fileData);
            if (!uploaded)
            {
                Console.Error.WriteLine("Failed to upload to coveralls");
                Environment.Exit(1);
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
