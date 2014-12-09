using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csmacnz.Coveralls
{
    public sealed class CoverallData
    {
        [JsonProperty("repo_token")]
        public string RepoToken { get; set; }

        [JsonProperty("service_job_id")]
        public string ServiceJobId { get; set; }

        [JsonProperty("service_name")]
        public string ServiceName { get; set; }

        [JsonProperty("source_files")]
        public CoverageFile[] SourceFiles { get; set; }
    }
}
