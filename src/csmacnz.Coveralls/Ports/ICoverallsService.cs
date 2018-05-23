using BCLExtensions;
using Beefeater;

namespace csmacnz.Coveralls.Ports
{
    public interface ICoverallsService
    {
        Result<Unit, string> Upload(string fileData);

        Result<Unit, string> PushParallelCompleteWebhook(string repoToken, string buildNumber);
    }
}
