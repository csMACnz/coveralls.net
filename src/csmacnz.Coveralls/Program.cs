using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Mono.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;

namespace csmacnz.Coveralls
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var p = new OptionSet();
            p.Parse(args);

            var fileName = @"opencovertests.xml";
            var document = XDocument.Load(fileName);

            List<CoverageFile> files = new OpenCoverParser(new FileSystem()).GenerateSourceFiles(document);

            GitData gitData = null;
            var commitId = Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT");
            if (!string.IsNullOrWhiteSpace(commitId))
            {
                gitData = new GitData
                {
                    Head = new GitHead
                    {
                        Id = commitId,
                        AuthorName = Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT_AUTHOR"),
                        AuthorEmail = Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL"),
                        CommitterName = Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT_AUTHOR"),
                        ComitterEmail = Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL"),
                        Message = Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT_MESSAGE")
                    },
                    Branch = Environment.GetEnvironmentVariable("APPVEYOR_REPO_BRANCH")
                };
            }

            var serviceJobId = Environment.GetEnvironmentVariable("APPVEYOR_JOB_ID") ?? "0";

            var data = new CoverallData
            {
                RepoToken = "UCIcRAOyPJIDrjvG8MreBKnKPonmR2L10",
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
