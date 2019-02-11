using csmacnz.Coveralls.Adapters;
using csmacnz.Coveralls.Data;
using csmacnz.Coveralls.Ports;

namespace csmacnz.Coveralls.GitDataResolvers
{
    public class GitlabGitDataResolver : IGitDataResolver
    {
        private readonly IEnvironmentVariables _variables;

        public GitlabGitDataResolver(IEnvironmentVariables variables)
        {
            _variables = variables;
        }

        public string DisplayName => "Gitlab Environment Variables";

        public bool CanProvideData()
        {
            return _variables.GetBooleanVariable("GITLAB_CI");
        }

        public GitData GenerateData()
        {
            var generateData = new GitData
            {
                Head = new GitHead
                {
                    Id = _variables.GetEnvironmentVariable("CI_COMMIT_SHA") ?? string.Empty,
                    AuthorName = _variables.GetEnvironmentVariable("GITLAB_USER_NAME") ?? string.Empty,
                    AuthorEmail = _variables.GetEnvironmentVariable("GITLAB_USER_EMAIL") ?? string.Empty,
                    CommitterName = _variables.GetEnvironmentVariable("GITLAB_USER_NAME") ?? string.Empty,
                    ComitterEmail =
                        _variables.GetEnvironmentVariable("GITLAB_USER_EMAIL") ?? string.Empty,
                    Message = _variables.GetEnvironmentVariable("CI_COMMIT_MESSAGE") ?? string.Empty
                },
                Branch = _variables.GetEnvironmentVariable("CI_COMMIT_REF_NAME") ?? string.Empty
            };

            return generateData;
        }
    }
}
