using Beefeater;

namespace csmacnz.Coveralls.DataAccess
{
    public interface ICoverallsService
    {
        Result<bool, string> Upload(string fileData);
    }
}