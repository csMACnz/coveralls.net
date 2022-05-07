namespace csmacnz.Coveralls.Ports;

public interface IEnvironmentVariables
{
    string? GetEnvironmentVariable(string key);
}

public static class EnvironmentVariablesExtensions
{
    public static bool GetBooleanVariable(this IEnvironmentVariables variables, string key)
    {
        if (variables is null)
        {
            throw new ArgumentNullException(paramName: nameof(variables));
        }

        return bool.TryParse(variables.GetEnvironmentVariable(key), out var result) && result;
    }
}
