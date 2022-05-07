namespace csmacnz.Coveralls.Data;

public sealed class CoverallData
{
    public CoverallData(
        string repoToken,
        string serviceName,
        CoverageFile[] sourceFiles)
    {
        RepoToken = repoToken;
        ServiceName = serviceName;
        SourceFiles = sourceFiles;
    }

    [JsonProperty("repo_token")]
    public string RepoToken { get; }

    [JsonProperty("service_job_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? ServiceJobId { get; set; }

    [JsonProperty("service_name")]
    public string ServiceName { get; }

    [JsonProperty("service_number", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? ServiceNumber { get; set; }

    [JsonProperty("service_pull_request", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? PullRequestId { get; set; }

    [JsonProperty("parallel", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool? Parallel { get; set; }

    [JsonProperty("source_files")]
    public CoverageFile[] SourceFiles { get; }

    [JsonProperty("git", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public GitData? Git { get; set; }

    [JsonProperty("commit_sha", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? CommitSha { get; set; }
}
