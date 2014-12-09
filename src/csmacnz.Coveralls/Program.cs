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

            var coverageBuilder = new CoverageFileBuilder(@"C:\sourceFile.cs");
            var file = coverageBuilder.CreateFile();

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
                        Message = Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT_MESSAGE")
                    },
                    Branch = Environment.GetEnvironmentVariable("APPVEYOR_REPO_BRANCH")
                };
            }

            var serviceJobId = Environment.GetEnvironmentVariable("APPVEYOR_JOB_ID") ?? "0";

            var data = new CoverallData
            {
                RepoToken = "9HuFWYElkcbOYDtBbEOpPgiTOkzKp4CG1",
                ServiceJobId = serviceJobId,
                ServiceName = "coveralls.net",
                SourceFiles = new[] { file },
                Git = gitData
            };

            var fileData = JsonConvert.SerializeObject(data);
            var uploaded = Upload(@"https://coveralls.io/api/v1/jobs", fileData);
            if (!uploaded)
            {
                Environment.Exit(1);
            }
        }

        private static bool Upload(string url, string fileData)
        {
            HttpContent stringContent = new StringContent(fileData);

            using (var client = new HttpClient())
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(stringContent, "json_file", "json_file");

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
