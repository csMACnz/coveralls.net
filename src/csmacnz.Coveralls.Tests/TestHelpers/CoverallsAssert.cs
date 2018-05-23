using System;
using Xunit;

namespace csmacnz.Coveralls.Tests.TestHelpers
{
    public static class CoverallsAssert
    {
        public static void RanSuccessfully(CoverallsRunResults results)
        {
            Assert.True(results.ExitCode == 0, $"Expected a Successful run but returned an exit code of {results.ExitCode}:\n{results.StandardError}");
        }

        public static void ContainsStandardUsageText(CoverallsRunResults results)
        {
            Assert.Contains("Usage:", results.StandardOutput, StringComparison.Ordinal);
            Assert.Contains("csmacnz.Coveralls --help", results.StandardOutput, StringComparison.Ordinal);
            Assert.Contains("Options:", results.StandardOutput, StringComparison.Ordinal);
            Assert.Contains("What it's for:", results.StandardOutput, StringComparison.Ordinal);
        }
    }
}
