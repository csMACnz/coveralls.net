namespace csmacnz.Coveralls;

public record ConfigurationSettings(
        string RepoToken,
        string? OutputFile,
        bool DryRun,
        bool TreatUploadErrorsAsWarnings,
        bool UseRelativePaths,
        string? BasePath,
        IReadOnlyList<CoverageSource> CoverageSources);
