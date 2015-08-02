using System;

namespace csmacnz.Coveralls
{
    internal class EnvironmentVariables : IEnvironmentVariables
    {
        public string GetEnvironmentVariable(string key)
        {
            return Environment.GetEnvironmentVariable(key);
        }
    }
}