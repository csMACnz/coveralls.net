using System;
using System.Net.Http;
using System.Text;
using BCLExtensions;
using Beefeater;
using csmacnz.Coveralls.Ports;
using Newtonsoft.Json;

namespace csmacnz.Coveralls.Adapters
{
    public class CoverallsService : ICoverallsService
    {
        private readonly Uri _baseUri;

#pragma warning disable S1075 // URIs should not be hardcoded
        private static readonly Uri JobsUri = new Uri("/api/v1/jobs", UriKind.Relative);
#pragma warning restore S1075 // URIs should not be hardcoded

        private static Uri WebhookUri(string repoToken) => new Uri($"/webhook?repo_token={System.Net.WebUtility.UrlEncode(repoToken)}", UriKind.Relative);

        public CoverallsService(Uri baseUri)
        {
            _baseUri = baseUri;
        }

        public Result<Unit, string> PushParallelCompleteWebhook(string repoToken, string? buildNumber)
        {
            var payload = $@"
{{
    ""payload"": {{
        ""build_num"": ""{buildNumber}"",
        ""status"": ""done""
    }}
}}";

            using HttpContent stringContent = new StringContent(payload, Encoding.Default, "application/json");
            using var client = new HttpClient();
            var url = new Uri(_baseUri, WebhookUri(repoToken));

            var response = client.PostAsync(url, stringContent).Result;

            var content = response.Content.ReadAsStringAsync().Result;
            if (!response.IsSuccessStatusCode)
            {
                var message = TryGetJsonMessageFromResponse(content).ValueOr(content);

                if (message.Length > 200)
                {
                    message = message.Substring(0, 200);
                }

                return $"{response.StatusCode} - {message}";
            }

            var error = TryFindErrorFromResponse(content);

            return error.Match<Result<Unit, string>>(
                e => $"{response.StatusCode} - {e}",
                () => Unit.Default);
        }

        public Result<Unit, string> Upload(string fileData)
        {
            using HttpContent stringContent = new StringContent(fileData);
            using HttpClient client = new HttpClient();
            using var formData = new MultipartFormDataContent
            {
                { stringContent, "json_file", "coverage.json" }
            };

            var response = client.PostAsync(new Uri(_baseUri, JobsUri), formData).Result;

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "TryGet is intentionally swallowing errors")]
        private static Option<string> TryGetJsonMessageFromResponse(string content)
        {
            try
            {
                object? obj = JsonConvert.DeserializeObject(content);
                if (obj is null)
                {
                    return Option<string>.None;
                }

                dynamic result = obj;
                return (string)result.message;
            }
            catch (Exception)
            {
                return Option<string>.None;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "TryFind is intentionally swallowing errors")]
        private static Option<string> TryFindErrorFromResponse(string content)
        {
            try
            {
                object? obj = JsonConvert.DeserializeObject(content);
                if (obj is null)
                {
                    return Option<string>.None;
                }

                dynamic result = obj;
                return new Option<string>((string)result.error);
            }
            catch (Exception)
            {
                return Option<string>.None;
            }
        }
    }
}
