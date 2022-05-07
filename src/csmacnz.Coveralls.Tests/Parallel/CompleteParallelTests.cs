using System;
using System.Collections.Generic;
using csmacnz.Coveralls.Tests.TestAdapters;
using csmacnz.Coveralls.Tests.TestHelpers;
using Xunit;

namespace csmacnz.Coveralls.Tests.Parallel;

public class CompleteParallelTests
{
    [Fact]
    public void CompleteParallelCommandWithTokenWorks()
    {
        var results = CoverallsTestRunner.RunCoveralls("--completeParallelWork --repoToken MYTESTREPOTOKEN");

        CoverallsAssert.RanSuccessfully(results);
    }

    [Fact]
    public void CompleteParallelCommandWithTokenVariableWorks()
    {
        var environment = new TestEnvironmentVariables(new Dictionary<string, string>
        {
            { "MYTESTREPOTOKENVAR", "MYTESTREPOTOKEN" }
        });

        var results = CoverallsTestRunner.RunCoveralls(
            "--completeParallelWork --repoTokenVariable MYTESTREPOTOKENVAR",
            testEnvironmentVariables: environment);

        CoverallsAssert.RanSuccessfully(results);
    }

    [Fact]
    public void CompleteParallelCommandWithServiceBuildNumberWorks()
    {
        var results = CoverallsTestRunner.RunCoveralls(
            "--completeParallelWork --repoToken MYTESTREPOTOKEN --serviceNumber 1234");

        CoverallsAssert.RanSuccessfully(results);
    }

    [Fact]
    public void CompleteParallelCommandWithEnvironmentVariablesWorks()
    {
        var environment = new TestEnvironmentVariables(new Dictionary<string, string>
        {
            { "COVERALLS_REPO_TOKEN", "MYTESTREPOTOKEN" },
            { "APPVEYOR_BUILD_NUMBER", "1234" }
        });

        var results = CoverallsTestRunner.RunCoveralls(
            "--completeParallelWork",
            testEnvironmentVariables: environment);

        CoverallsAssert.RanSuccessfully(results);
    }

    [Fact]
    public void InvalidArgument_ExitCodeNotSuccess()
    {
        var results = CoverallsTestRunner.RunCoveralls("--completeParallelWork --notanoption");

        Assert.NotEqual(0, results.ExitCode);
        Assert.Contains("Usage:", results.StandardOutput, StringComparison.Ordinal);
    }
}
