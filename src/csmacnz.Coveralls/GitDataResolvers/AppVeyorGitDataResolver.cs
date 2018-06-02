using csmacnz.Coveralls.Adapters;
using csmacnz.Coveralls.Data;
using csmacnz.Coveralls.Ports;

namespace csmacnz.Coveralls.GitDataResolvers
{
    public class AppVeyorGitDataResolver : IGitDataResolver
    {
        private readonly IEnvironmentVariables _variables;

        public AppVeyorGitDataResolver(IEnvironmentVariables variables)
        {
            _variables = variables;
        }

        public string DisplayName => "AppVeyor Environment Variables";

        public bool CanProvideData()
        {
            return _variables.GetBooleanVariable("APPVEYOR");
        }

        public GitData GenerateData()
        {
            var generateData = new GitData
            {
                Head = new GitHead
                {
                    Id = _variables.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT") ?? string.Empty,
                    AuthorName = _variables.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT_AUTHOR") ?? string.Empty,
                    AuthorEmail = _variables.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL") ?? string.Empty,
                    CommitterName = _variables.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT_AUTHOR") ?? string.Empty,
                    ComitterEmail =
                        _variables.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL") ?? string.Empty,
                    Message = _variables.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT_MESSAGE") ?? string.Empty
                },
                Branch = _variables.GetEnvironmentVariable("APPVEYOR_REPO_BRANCH") ?? string.Empty
            };

            return generateData;
        }
    }
}
