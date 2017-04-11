namespace csmacnz.Coveralls.Ports
{
    public interface IEnvironmentVariables
    {
        string GetEnvironmentVariable(string key);
    }
}