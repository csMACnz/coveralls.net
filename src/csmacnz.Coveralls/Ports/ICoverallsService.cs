namespace csmacnz.Coveralls.Ports;

public interface ICoverallsService
{
    Result<Unit, string> Upload(string fileData, Uri serverUrl);

    Result<Unit, string> PushParallelCompleteWebhook(string repoToken, string? buildNumber, Uri serverUrl);
}
