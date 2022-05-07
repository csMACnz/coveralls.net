using System;
using Xunit;

namespace csmacnz.Coveralls.Tests.Integration;

public static class CoverallsAssert
{
    public static void RanSuccessfully(CoverallsRunResults results)
    {
        _ = results ?? throw new ArgumentNullException(nameof(results));

        Assert.True(results.ExitCode == 0, $"Expected a Successful run but returned an exit code of {results.ExitCode}:\n{results.StandardError}");
    }
}
