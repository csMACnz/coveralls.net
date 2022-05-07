namespace csmacnz.Coveralls.Adapters;

internal class EnvironmentVariables : IEnvironmentVariables
{
    public string? GetEnvironmentVariable(string key)
        => Environment.GetEnvironmentVariable(key);
}
