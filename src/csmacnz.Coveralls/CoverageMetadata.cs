namespace csmacnz.Coveralls
{
    public class CoverageMetadata
    {
        public string RepoToken { get; set; }
        public string ServiceName { get; set; }
        public string ServiceJobId { get; set; }
        public string ServiceNumber { get; set; }
        public string PullRequestId { get; set; }
        public bool Parallel { get; set; }
    }
}