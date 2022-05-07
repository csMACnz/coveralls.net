namespace csmacnz.Coveralls.Data;

public sealed class GitRemotes
{
    [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? Name { get; set; }

    [JsonProperty("url", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? Url { get; set; }
}
