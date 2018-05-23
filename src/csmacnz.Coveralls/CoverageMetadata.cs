namespace csmacnz.Coveralls
{
    public class CoverageMetadata
    {
        public string ServiceName { get; set; }

        public string ServiceJobId { get; set; }

        public string ServiceBuildNumber { get; set; }

        public string PullRequestId { get; set; }

        public bool Parallel { get; set; }
    }
}
