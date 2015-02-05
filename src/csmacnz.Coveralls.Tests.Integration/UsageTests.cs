using System.Text.RegularExpressions;
using Xunit;
using Xunit.Should;

namespace csmacnz.Coveralls.Tests.Integration
{
    public class UsageTests
    {
        [Fact]
        public void NoArguments_ExitCodeNotSuccess()
        {
            var results = CoverallsTestRunner.RunCoveralls(string.Empty);

            results.ExitCode.ShouldNotBe(0);
        }
        [Fact]
        public void InvalidArgument_ExitCodeNotSuccess()
        {
            var results = CoverallsTestRunner.RunCoveralls("--notanoption");

            results.ExitCode.ShouldNotBe(0);
        }

        [Fact]
        public void FileDoesntExist()
        {
            var results = CoverallsTestRunner.RunCoveralls("--opencover -i opencover.xml --dryrun --repoToken MYTESTREPOTOKEN");

            results.ExitCode.ShouldNotBe(0);
            results.StandardError.ShouldContain("Input file 'opencover.xml' cannot be found");
        }
        [Fact]
        public void Version()
        {
            var results = CoverallsTestRunner.RunCoveralls("--version");

            Assert.True(Regex.IsMatch(results.StandardOutput, @"\d+.\d+.\d+.\d+"));
        }

        [Fact]
        public void HelpLongHand()
        {
            var results = CoverallsTestRunner.RunCoveralls("--help");

            results.ExitCode.ShouldBe(0);
            results.StandardOutput.ShouldContain("Usage:");
            results.StandardOutput.ShouldContain("csmacnz.Coveralls --help");
            results.StandardOutput.ShouldContain("Options:");
            results.StandardOutput.ShouldContain("Options:");
            results.StandardOutput.ShouldContain("What its for:");
        }

        [Fact]
        public void HelpShortHand()
        {
            var results = CoverallsTestRunner.RunCoveralls("-h");

            results.ExitCode.ShouldBe(0);
            results.StandardOutput.ShouldContain("Usage:");
            results.StandardOutput.ShouldContain("csmacnz.Coveralls --help");
            results.StandardOutput.ShouldContain("Options:");
            results.StandardOutput.ShouldContain("Options:");
            results.StandardOutput.ShouldContain("What its for:");
        }

    }
}
