namespace csmacnz.Coveralls
{
    public class CoverageMetadata
    {
        public CoverageMetadata(
            string serviceName,
            string serviceJobId,
            string? serviceBuildNumber,
            string? pullRequestId,
            bool parallel)
        {
            (ServiceName, ServiceJobId, ServiceBuildNumber, PullRequestId, Parallel)
            = (serviceName, serviceJobId, serviceBuildNumber, pullRequestId, parallel);
        }

        public string ServiceName { get; }

        public string ServiceJobId { get; }

        public string? ServiceBuildNumber { get; }

        public string? PullRequestId { get; }

        public bool Parallel { get; }
    }
}
