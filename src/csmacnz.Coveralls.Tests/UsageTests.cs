using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using csmacnz.Coveralls.Tests.TestAdapters;
using csmacnz.Coveralls.Tests.TestHelpers;
using Xunit;

namespace csmacnz.Coveralls.Tests;

public class UsageTests
{
    [Fact]
    public void NoArguments_ExitCodeNotSuccess()
    {
        var results = CoverallsTestRunner.RunCoveralls(string.Empty);

        Assert.NotEqual(0, results.ExitCode);
    }

    [Fact]
    public void InvalidArgument_ExitCodeNotSuccess()
    {
        var results = CoverallsTestRunner.RunCoveralls("--notanoption");

        Assert.NotEqual(0, results.ExitCode);
    }

    [Theory]
    [InlineData("w")]
    [InlineData("coveralls.io")] // Needs http://
    public void InvalidServerUrl_FailsWithCorrectError(string badUrl)
    {
        var results = CoverallsTestRunner.RunCoveralls($"--opencover -i anytestfile.xml --dryrun --repoToken MYTESTREPOTOKEN --serverUrl {badUrl}");

        Assert.NotEqual(0, results.ExitCode);
        Assert.Contains($"Invalid --serverUrl ({badUrl}) provided.", results.StandardError, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("ftp://coveralls.io", "ftp")] // Needs http or https
    [InlineData("tcp://coveralls.io", "tcp")] // Needs http or https
    public void InvalidServerUrlSchema_FailsWithCorrectError(string badUrl, string scheme)
    {
        var results = CoverallsTestRunner.RunCoveralls($"--opencover -i anytestfile.xml --dryrun --repoToken MYTESTREPOTOKEN --serverUrl {badUrl}");

        Assert.NotEqual(0, results.ExitCode);
        Assert.Contains($"Invalid --serverUrl scheme ({scheme}) provided. ('http' and 'https' supported only)", results.StandardError, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("/api/v3")]
    [InlineData("/root")]
    public void UnixFilePathUrlSchema_FailsWithCorrectError(string badUrl)
    {
        var results = CoverallsTestRunner.RunCoveralls($"--opencover -i anytestfile.xml --dryrun --repoToken MYTESTREPOTOKEN --serverUrl {badUrl}");

        Assert.NotEqual(0, results.ExitCode);
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // On windows this is a relative path
            Assert.Contains($"Invalid --serverUrl ({badUrl}) provided.", results.StandardError, StringComparison.Ordinal);
        }
        else
        {
            // On Unix this is a file path
            Assert.Contains($"Invalid --serverUrl scheme (file) provided. ('http' and 'https' supported only)", results.StandardError, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void FileDoesntExist()
    {
        var results =
            CoverallsTestRunner.RunCoveralls("--opencover -i opencover.xml --dryrun --repoToken MYTESTREPOTOKEN");

        Assert.NotEqual(0, results.ExitCode);
        Assert.Contains("Input file 'opencover.xml' cannot be found", results.StandardError, StringComparison.Ordinal);
    }

    [Fact]
    public void Version()
    {
        var results = CoverallsTestRunner.RunCoveralls("--version");

        Assert.True(
            Regex.IsMatch(results.StandardOutput, @"\d+.\d+.\d+.\d+"),
            "Version doesn't match regex: " + results.StandardOutput);
    }

    [Fact]
    public void HelpLongHand()
    {
        var results = CoverallsTestRunner.RunCoveralls("--help");

        CoverallsAssert.RanSuccessfully(results);
        CoverallsAssert.ContainsStandardUsageText(results);
    }

    [Fact]
    public void HelpShortHand()
    {
        var results = CoverallsTestRunner.RunCoveralls("-h");

        CoverallsAssert.RanSuccessfully(results);
        CoverallsAssert.ContainsStandardUsageText(results);
    }
}
