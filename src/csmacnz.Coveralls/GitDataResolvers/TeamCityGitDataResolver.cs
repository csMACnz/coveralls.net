using BCLExtensions;
using Beefeater;
using csmacnz.Coveralls.Data;
using csmacnz.Coveralls.Ports;

namespace csmacnz.Coveralls.GitDataResolvers
{
    public class TeamCityGitDataResolver : IGitDataResolver
    {
        private readonly IEnvironmentVariables _variables;
        private readonly IConsole _console;

        public TeamCityGitDataResolver(IEnvironmentVariables variables, IConsole console)
        {
            _variables = variables;
            _console = console;
        }

        public string DisplayName => "Teamcity Environment Variables";

        public bool CanProvideData()
        {
            return _variables.GetBooleanVariable("TEAMCITY_VERSION");
        }

        public Either<GitData, CommitSha> GenerateData()
        {
            var customVariablesSha = _variables.GetEnvironmentVariable("TEAMCITY_BUILD_COMMIT");

            if (customVariablesSha.IsNotNullOrWhitespace())
            {
                var generateData = new GitData
                {
                    Head = new GitHead
                    {
                        Id = customVariablesSha,
                    },
                    Branch = _variables.GetEnvironmentVariable("TEAMCITY_BUILD_BRANCH") ?? string.Empty
                };

                return generateData;
            }
            else
            {
                _console.WriteLine("Teamcity does not automatically make build parameters available as environment variables.");
                _console.WriteLine("Add the following environment parameters to the build configuration");
                _console.WriteLine("env.TEAMCITY_BUILD_BRANCH = %teamcity.build.vcs.branch.<YOUR TEAMCITY VCS NAME>%");
                _console.WriteLine("env.TEAMCITY_BUILD_COMMIT = %build.vcs.number%");
                _console.WriteLine("env.TEAMCITY_BUILD_NUMBER = %build.counter%");
                _console.WriteLine("env.TEAMCITY_PULL_REQUEST = %teamcity.build.branch%");
                var noCustomVariablesSha = _variables.GetEnvironmentVariable("BUILD_VCS_NUMBER");
                if (noCustomVariablesSha.IsNotNullOrWhitespace())
                {
                    _console.WriteLine("Using env.BUILD_VCS_NUMBER to submit commit sha only");
                    return new CommitSha(noCustomVariablesSha);
                }
                else
                {
                    return Either<GitData, CommitSha>.OfResult1(null);
                }
            }
        }
    }
}
