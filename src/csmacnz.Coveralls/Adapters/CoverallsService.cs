namespace csmacnz.Coveralls.Adapters;

public class CoverallsService : ICoverallsService
{
    private static readonly Uri JobsUri = new("/api/v1/jobs", UriKind.Relative);

    private static Uri WebhookUri(string repoToken, string? carryForward)
    {
        var uriBuilder = new UriBuilder(new Uri("/webhook", UriKind.Relative));
        var query = System.Web.HttpUtility.ParseQueryString(string.Empty);

        query.Add("repo_token", repoToken);

        if (!string.IsNullOrWhiteSpace(carryForward))
        {
            query.Add("carryforward", carryForward);
        }

        uriBuilder.Query = query.ToString();
        return uriBuilder.Uri;
    }

    public Result<Unit, string> PushParallelCompleteWebhook(string repoToken, string? buildNumber, Uri serverUrl, string? carryForward)
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
        var url = new Uri(serverUrl, WebhookUri(repoToken, carryForward));

        var response = client.PostAsync(url, stringContent).Result;

        var content = response.Content.ReadAsStringAsync().Result;
        if (!response.IsSuccessStatusCode)
        {
            var message = TryGetJsonMessageFromResponse(content).ValueOr(content);

            if (message.Length > 200)
            {
                message = message[..200];
            }

            return $"{response.StatusCode} - {message}";
        }

        var error = TryFindErrorFromResponse(content);

        return error.Match<Result<Unit, string>>(
            e => $"{response.StatusCode} - {e}",
            () => Unit.Default);
    }

    public Result<Unit, string> Upload(string fileData, Uri serverUrl)
    {
        using HttpContent stringContent = new StringContent(fileData);
        using HttpClient client = new();
        using var formData = new MultipartFormDataContent
        {
            { stringContent, "json_file", "coverage.json" }
        };

        var boundary = formData.Headers.ContentType?.Parameters.FirstOrDefault(o => o.Name == "boundary");
        if (boundary != null)
        {
            boundary.Value = boundary.Value?.Replace("\"", string.Empty);
        }

        var response = client.PostAsync(new Uri(serverUrl, JobsUri), formData).Result;

        if (!response.IsSuccessStatusCode)
        {
            var content = response.Content.ReadAsStringAsync().Result;
            var message = TryGetJsonMessageFromResponse(content).ValueOr(content);

            if (message.Length > 200)
            {
                message = message[..200];
            }

            return $"{response.StatusCode} - {message}";
        }

        return Unit.Default;
    }

    private static Option<string> TryGetJsonMessageFromResponse(string content)
    {
        try
        {
            var obj = JsonConvert.DeserializeObject(content);
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

    private static Option<string> TryFindErrorFromResponse(string content)
    {
        try
        {
            var obj = JsonConvert.DeserializeObject(content);
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
