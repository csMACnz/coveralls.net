using Beefeater;

namespace csmacnz.Coveralls.Ports
{
    public interface IEnvironmentVariables
    {
        string GetEnvironmentVariable(string key);
    }

    public static class EnvironmentVariablesExtensions
    {
        public static bool GetBooleanVariable(this IEnvironmentVariables variables, string key)
        {
            return bool.TryParse(variables.GetEnvironmentVariable(key), out var result) && result;
        }
    }
}
