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

            var data = new CoverallData
            {
                RepoToken = "9HuFWYElkcbOYDtBbEOpPgiTOkzKp4CG1",
                ServiceJobId = "0",
                ServiceName = "coveralls.net",
                SourceFiles = new[] { file }
            };

            var fileData = JsonConvert.SerializeObject(data);
            Upload(@"https://coveralls.io/api/v1/jobs", fileData);
        }

        private static System.IO.Stream Upload(string url, string filename)
        {
            HttpContent stringContent = new StringContent(filename);
            
            using (var client = new HttpClient())
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(stringContent, "json_file", "json_file");
                
                var response = client.PostAsync(url, formData).Result;

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }
                return response.Content.ReadAsStreamAsync().Result;
            }
        }
    }
}
