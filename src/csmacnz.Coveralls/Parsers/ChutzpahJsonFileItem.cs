namespace csmacnz.Coveralls.Parsers
{
    public class ChutzpahJsonFileItem
    {
        public string FilePath { get; set; }

        public int?[] LineExecutionCounts { get; set; }

        public string[] SourceLines { get; set; }

        public double CoveragePercentage { get; set; }
    }
}