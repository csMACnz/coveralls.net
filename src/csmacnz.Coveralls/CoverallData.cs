using Newtonsoft.Json;

namespace csmacnz.Coveralls
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
    }

    public sealed class GitData
    {
        [JsonProperty("head", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public GitHead Head { get; set; }

        [JsonProperty("branch", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Branch { get; set; }

        [JsonProperty("remotes", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public GitRemotes Remotes { get; set; }
    }

    public sealed class GitHead
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("author_name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string AuthorName { get; set; }

        [JsonProperty("author_email", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string AuthorEmail { get; set; }

        [JsonProperty("committer_name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string CommitterName { get; set; }

        [JsonProperty("committer_email", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ComitterEmail { get; set; }

        [JsonProperty("message", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Message { get; set; }

    }

    public sealed class GitRemotes
    {
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("url", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Url { get; set; }
    }
}
