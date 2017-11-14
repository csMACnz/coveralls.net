using System.Collections.Generic;

namespace csmacnz.Coveralls
{
    public class ConfigurationSettings
    {
        public bool DryRun { get; set; }
        public bool TreatUploadErrorsAsWarnings { get; set; }
        public bool UseRelativePaths { get; set; }
        public string OutputFile { get; set; }
        public string BasePath { get; set; }
        public List<CoverageSource> CoverageSources { get; set; }
        public string RepoToken { get; set; }
    }
}