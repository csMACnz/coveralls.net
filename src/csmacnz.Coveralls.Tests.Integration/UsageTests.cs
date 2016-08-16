using System.Text.RegularExpressions;
using Xunit;

namespace csmacnz.Coveralls.Tests.Integration
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
            var results = CoverallsTestRunner.RunCoveralls("--opencover -i opencover.xml --dryrun --repoToken MYTESTREPOTOKEN");

            Assert.NotEqual(0, results.ExitCode);
            Assert.Contains("Input file 'opencover.xml' cannot be found", results.StandardError);
        }
        [Fact]
        public void Version()
        {
            var results = CoverallsTestRunner.RunCoveralls("--version");

            Assert.True(Regex.IsMatch(results.StandardOutput, @"\d+.\d+.\d+.\d+"), "Version doesn't match regex: " + results.StandardOutput);
        }

        [Fact]
        public void HelpLongHand()
        {
            var results = CoverallsTestRunner.RunCoveralls("--help");

            Assert.Equal(0, results.ExitCode);
            ContainsStandardUsageText(results);
        }

        [Fact]
        public void HelpShortHand()
        {
            var results = CoverallsTestRunner.RunCoveralls("-h");

            Assert.Equal(0, results.ExitCode);
            ContainsStandardUsageText(results);
        }

        private static void ContainsStandardUsageText(CoverageRunResults results)
        {
            Assert.Contains("Usage:", results.StandardOutput);
            Assert.Contains("csmacnz.Coveralls --help", results.StandardOutput);
            Assert.Contains("Options:", results.StandardOutput);
            Assert.Contains("Options:", results.StandardOutput);
            Assert.Contains("What it's for:", results.StandardOutput);
        }
    }
}
