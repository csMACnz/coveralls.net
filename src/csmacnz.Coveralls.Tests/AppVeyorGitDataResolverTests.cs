using System.Collections.Generic;
using Xunit;

namespace csmacnz.Coveralls.Tests
{
    public class AppVeyorGitDataResolverTests
    {
        [Fact]
        public void CanProvideDataNoEnvironmentVariablesSetReturnsFalse()
        {
            var sut = new AppVeyorGitDataResolver(new TestEnvironmentVariables(new Dictionary<string, string>()));

            var canProvideData = sut.CanProvideData();

            Assert.False(canProvideData);
        }

        [Fact]
        public void CanProvideDataAppVeyorEnvironmentVariableSetToFalseReturnsFalse()
        {
            IEnvironmentVariables variables = new TestEnvironmentVariables(new Dictionary<string, string>
            {
                {"APPVEYOR", "False"}
            });

            var sut = new AppVeyorGitDataResolver(variables);

            var canProvideData = sut.CanProvideData();

            Assert.False(canProvideData);
        }

        [Fact]
        public void CanProvideDataAppVeyorEnvironmentVariablesSetReturnsTrue()
        {
            IEnvironmentVariables variables = new TestEnvironmentVariables(new Dictionary<string, string>
            {
                {"APPVEYOR", "True"}
            });

            var sut = new AppVeyorGitDataResolver(variables);

            var canProvideData = sut.CanProvideData();

            Assert.True(canProvideData);
        }
    }

    public class TestEnvironmentVariables : IEnvironmentVariables
    {
        private Dictionary<string, string> _variables;

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
