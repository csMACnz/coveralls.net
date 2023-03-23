namespace csmacnz.Coveralls;

public record CoverageMetadata(
        string ServiceName,
        string ServiceJobId,
        string? ServiceBuildNumber,
        string? PullRequestId,
        bool Parallel,
        string? CarryForward);
