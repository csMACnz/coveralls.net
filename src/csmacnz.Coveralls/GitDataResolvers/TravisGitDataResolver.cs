namespace csmacnz.Coveralls.GitDataResolvers;

public class TravisGitDataResolver : IGitDataResolver
{
    private readonly IEnvironmentVariables _variables;

    public TravisGitDataResolver(IEnvironmentVariables variables) => _variables = variables;

    public string DisplayName => "Travis Environment Variables";

    public bool CanProvideData() => _variables.GetBooleanVariable("TRAVIS");

    public Either<GitData, CommitSha>? GenerateData()
    {
        var generateData = new GitData
        {
            Head = new GitHead
            {
                Id = _variables.GetEnvironmentVariable("TRAVIS_COMMIT") ?? string.Empty,
                AuthorName = _variables.GetEnvironmentVariable("REPO_COMMIT_AUTHOR") ?? string.Empty,
                AuthorEmail = _variables.GetEnvironmentVariable("REPO_COMMIT_AUTHOR_EMAIL") ?? string.Empty,
                CommitterName = _variables.GetEnvironmentVariable("REPO_COMMIT_AUTHOR") ?? string.Empty,
                ComitterEmail =
                    _variables.GetEnvironmentVariable("REPO_COMMIT_AUTHOR_EMAIL") ?? string.Empty,
                Message = _variables.GetEnvironmentVariable("REPO_COMMIT_MESSAGE") ?? string.Empty
            },
            Branch = _variables.GetEnvironmentVariable("TRAVIS_BRANCH") ?? string.Empty
        };

        return generateData;
    }
}
