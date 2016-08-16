using System;
using System.Net.Http;
using Beefeater;
using Newtonsoft.Json;

namespace csmacnz.Coveralls.DataAccess
{
    public class CoverallsService : ICoverallsService
    {
        private static readonly string RequestUri = @"https://coveralls.io/api/v1/jobs";

        //TODO(csMACnz): change from bool to Unit or a simplified Result<TError> as a thing?
        public Result<bool, string> Upload(string fileData)
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
                return (string)result.message;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}