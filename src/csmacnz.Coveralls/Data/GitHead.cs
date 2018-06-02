using Newtonsoft.Json;

namespace csmacnz.Coveralls.Data
{
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
}