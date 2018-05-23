using BCLExtensions;
using Beefeater;
using csmacnz.Coveralls.Data;

namespace csmacnz.Coveralls.GitDataResolvers
{
    public class CommandLineGitDataResolver : IGitDataResolver
    {
        private readonly MainArgs _args;

        public CommandLineGitDataResolver(MainArgs args)
        {
            _args = args;
        }

        public string DisplayName => "Command line Arguments";

        public bool CanProvideData()
        {
            var commitId = _args.OptCommitid;
            return commitId.IsNotNullOrWhitespace();
        }

        public Either<GitData, CommitSha> GenerateData()
        {
            GitData gitData = null;
            var commitId = _args.OptCommitid;
            if (commitId.IsNotNullOrWhitespace())
            {
                var committerName = _args.OptCommitauthor ?? string.Empty;
                var comitterEmail = _args.OptCommitemail ?? string.Empty;
                var commitMessage = _args.OptCommitmessage ?? string.Empty;
                gitData = new GitData
                {
                    Head = new GitHead
                    {
                        Id = commitId,
                        AuthorName = committerName,
                        AuthorEmail = comitterEmail,
                        CommitterName = committerName,
                        ComitterEmail = comitterEmail,
                        Message = commitMessage
                    },
                    Branch = _args.OptCommitbranch ?? string.Empty
                };
            }

            return gitData;
        }
    }
}
