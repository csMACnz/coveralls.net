using Newtonsoft.Json;

namespace csmacnz.Coveralls.Data
{
    public sealed class CoverallData
    {
        [JsonProperty("repo_token")]
        public string RepoToken { get; set; }

        [JsonProperty("service_job_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ServiceJobId { get; set; }

        [JsonProperty("service_name")]
        public string ServiceName { get; set; }

        [JsonProperty("service_number", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ServiceNumber { get; set; }

        [JsonProperty("service_pull_request", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string PullRequestId { get; set; }

        [JsonProperty("parallel", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? Parallel { get; set; }

        [JsonProperty("source_files")]
        public CoverageFile[] SourceFiles { get; set; }

        [JsonProperty("git", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public GitData Git { get; set; }

        [JsonProperty("commit_sha", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string CommitSha { get; set; }
    }
}
