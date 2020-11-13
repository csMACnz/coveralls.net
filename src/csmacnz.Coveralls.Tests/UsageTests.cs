using System;
using System.Text.RegularExpressions;
using csmacnz.Coveralls.Tests.TestHelpers;
using Xunit;

namespace csmacnz.Coveralls.Tests
{
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
}
