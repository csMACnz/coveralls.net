using BCLExtensions;
using Beefeater;
using csmacnz.Coveralls.Data;
using csmacnz.Coveralls.Ports;

namespace csmacnz.Coveralls.MetaDataResolvers
{
    public class TravisMetaDataResolver : IMetaDataResolver
    {
        private readonly IEnvironmentVariables _variables;

        public TravisMetaDataResolver(IEnvironmentVariables variables)
        {
            _variables = variables;
        }

        public bool IsActive()
        {
            return bool.TryParse(_variables.GetEnvironmentVariable("TRAVIS"), out var result) && result;
        }

        public Option<string> ResolveServiceName()
        {
            return "travis";
        }

        public Option<string> ResolveServiceJobId()
        {
            return GetFromVariable("TRAVIS_JOB_ID");
        }

        public Option<string> ResolveServiceNumber()
        {
            return GetFromVariable("TRAVIS_BUILD_NUMBER");
        }

        public Option<string> ResolvePullRequestId()
        {
            var value = GetFromVariable("TRAVIS_PULL_REQUEST");
            return value.Match(
                val => val == "false" ? Option<string>.None : val,
                () => Option<string>.None);
        }

        private Option<string> GetFromVariable(string variableName)
        {
            var prId = _variables.GetEnvironmentVariable(variableName);

            if (prId.IsNotNullOrWhitespace())
            {
                return prId;
            }

            return Option<string>.None;
        }
    }
}
