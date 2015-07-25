using Beefeater;

namespace csmacnz.Coveralls
{
    public interface ICoverallsService
    {
        Result<bool, string> Upload(string fileData);
    }
}