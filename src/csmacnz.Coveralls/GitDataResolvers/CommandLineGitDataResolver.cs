using BCLExtensions;
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

        public bool CanProvideData()
        {
            var commitId = _args.IsProvided("--commitId") ? _args.OptCommitid : string.Empty;
            return commitId.IsNotNullOrWhitespace();
        }

        public GitData GenerateData()
        {
            GitData gitData = null;
            var commitId = _args.IsProvided("--commitId") ? _args.OptCommitid : string.Empty;
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