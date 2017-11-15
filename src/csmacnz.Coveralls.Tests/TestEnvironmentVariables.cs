using System.Collections.Generic;
using csmacnz.Coveralls.Ports;

namespace csmacnz.Coveralls.Tests
{
    public class TestEnvironmentVariables : IEnvironmentVariables
    {
        private readonly Dictionary<string, string> _variables;

        public TestEnvironmentVariables(Dictionary<string, string> variables)
        {
            _variables = variables;
        }

        public string GetEnvironmentVariable(string key)
        {
            return _variables.ContainsKey(key) ? _variables[key] : string.Empty;
        }
    }
}
