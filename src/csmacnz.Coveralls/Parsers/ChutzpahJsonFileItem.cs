namespace csmacnz.Coveralls.Parsers
{
    public class ChutzpahJsonFileItem
    {
        public string FilePath { get; set; } = default!;

        public int?[] LineExecutionCounts { get; set; } = default!;

        public string[] SourceLines { get; set; } = default!;

        public double CoveragePercentage { get; set; }
    }
}
