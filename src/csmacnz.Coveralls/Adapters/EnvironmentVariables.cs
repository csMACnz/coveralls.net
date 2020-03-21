using System;
using csmacnz.Coveralls.Ports;

namespace csmacnz.Coveralls.Adapters
{
    internal class EnvironmentVariables : IEnvironmentVariables
    {
        public string? GetEnvironmentVariable(string key)
        {
            return Environment.GetEnvironmentVariable(key);
        }
    }
}
