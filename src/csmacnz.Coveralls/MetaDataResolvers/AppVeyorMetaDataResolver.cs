using BCLExtensions;
using Beefeater;
using csmacnz.Coveralls.Data;
using csmacnz.Coveralls.Ports;

namespace csmacnz.Coveralls.MetaDataResolvers
{
    public class AppVeyorMetaDataResolver : IMetaDataResolver
    {
        private readonly IEnvironmentVariables _variables;

        public AppVeyorMetaDataResolver(IEnvironmentVariables variables)
        {
            _variables = variables;
        }

        public bool IsActive()
        {
            return _variables.GetEnvironmentVariable("APPVEYOR");
        }

        public Option<string> ResolveServiceName()
        {
            return "appveyor";
        }

        public Option<string> ResolveServiceJobId()
        {
            return GetFromVariable("APPVEYOR_JOB_ID");
        }

        public Option<string> ResolveServiceNumber()
        {
            return GetFromVariable("APPVEYOR_BUILD_NUMBER");
        }

        public Option<string> ResolvePullRequestId()
        {
            return GetFromVariable("APPVEYOR_PULL_REQUEST_NUMBER");
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
