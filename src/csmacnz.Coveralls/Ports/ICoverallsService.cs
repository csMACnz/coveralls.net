using Beefeater;

namespace csmacnz.Coveralls.Ports
{
    public interface ICoverallsService
    {
        Result<bool, string> Upload(string fileData);
    }
}