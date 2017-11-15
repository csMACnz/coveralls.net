using System;
using System.Net.Http;
using BCLExtensions;
using Beefeater;
using csmacnz.Coveralls.Ports;
using Newtonsoft.Json;

namespace csmacnz.Coveralls.Adapters
{
    public class CoverallsService : ICoverallsService
    {
        // TODO(markc): make this configurable, especially if private servers are used.
#pragma warning disable S1075 // URIs should not be hardcoded
        private static readonly Uri RequestUri = new Uri("https://coveralls.io/api/v1/jobs");
#pragma warning restore S1075 // URIs should not be hardcoded

        public Result<Unit, string> Upload(string fileData)
        {
            using (HttpContent stringContent = new StringContent(fileData))
            {
                using (var client = new HttpClient())
                using (var formData = new MultipartFormDataContent())
                {
                    formData.Add(stringContent, "json_file", "coverage.json");

                    var response = client.PostAsync(RequestUri, formData).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        var message = TryGetJsonMessageFromResponse(content).ValueOr(content);

                        if (message.Length > 200)
                        {
                            message = message.Substring(0, 200);
                        }

                        return $"{response.StatusCode} - {message}";
                    }

                    return Unit.Default;
                }
            }
        }

        private static Option<string> TryGetJsonMessageFromResponse(string content)
        {
            try
            {
                dynamic result = JsonConvert.DeserializeObject(content);
                return (string)result.message;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
