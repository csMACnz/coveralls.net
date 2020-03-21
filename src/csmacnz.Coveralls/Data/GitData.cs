using Newtonsoft.Json;

namespace csmacnz.Coveralls.Data
{
    public sealed class GitData
    {
        [JsonProperty("head", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public GitHead? Head { get; set; }

        [JsonProperty("branch", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string? Branch { get; set; }

        [JsonProperty("remotes", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public GitRemotes? Remotes { get; set; }
    }
}
