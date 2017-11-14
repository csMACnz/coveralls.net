using csmacnz.Coveralls.Data;

namespace csmacnz.Coveralls
{
    public class CoverageSource
    {
        public CoverageMode Mode { get; }
        public string Input { get; }

        public CoverageSource(CoverageMode mode, string input)
        {
            Mode = mode;
            Input = input;
        }
    }
}