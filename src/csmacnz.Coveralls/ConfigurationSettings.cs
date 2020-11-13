using System.Collections.Generic;

namespace csmacnz.Coveralls
{
    public class ConfigurationSettings
    {
        public ConfigurationSettings(
            string repoToken,
            string? outputFile,
            bool dryRun,
            bool treatUploadErrorsAsWarnings,
            bool useRelativePaths,
            string? basePath,
            List<CoverageSource> coverageSources)
        {
            (DryRun, TreatUploadErrorsAsWarnings, UseRelativePaths, OutputFile, BasePath, CoverageSources, RepoToken)
            = (dryRun, treatUploadErrorsAsWarnings, useRelativePaths, outputFile, basePath, coverageSources, repoToken);
        }

        public bool DryRun { get; }

        public bool TreatUploadErrorsAsWarnings { get; }

        public bool UseRelativePaths { get; }

        public string? OutputFile { get; }

        public string? BasePath { get; }

        public IReadOnlyList<CoverageSource> CoverageSources { get; }

        public string RepoToken { get; }
    }
}
