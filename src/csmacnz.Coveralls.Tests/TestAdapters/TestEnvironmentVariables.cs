using System.Collections.Generic;
using csmacnz.Coveralls.Ports;

namespace csmacnz.Coveralls.Tests.TestAdapters;

public class TestEnvironmentVariables : IEnvironmentVariables
{
    private readonly Dictionary<string, string> _variables;

    public TestEnvironmentVariables() => _variables = new Dictionary<string, string>();

    public TestEnvironmentVariables(Dictionary<string, string> variables) => _variables = variables;

    public string GetEnvironmentVariable(string key) => _variables.ContainsKey(key) ? _variables[key] : string.Empty;
}
