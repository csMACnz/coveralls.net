using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Should;

namespace csmacnz.Coveralls.Tests.Integration
{
    public class UsageTests
    {
        private const string CoverallsExe = "csmacnz.Coveralls.exe";

        [Fact]
        public void Version()
        {
            var results = RunCoveralls("--version");

            Assert.True(Regex.IsMatch(results, @"\d+.\d+.\d+.\d+"));
        }

        [Fact]
        public void HelpLongHand()
        {
            var results = RunCoveralls("--help");

            results.ShouldContain("Usage:");
            results.ShouldContain("csmacnz.Coveralls --help");
            results.ShouldContain("Options:");
            results.ShouldContain("Options:");
            results.ShouldContain("What its for:");
        }

        [Fact]
        public void HelpShortHand()
        {
            var results = RunCoveralls("-h");

            results.ShouldContain("Usage:");
            results.ShouldContain("csmacnz.Coveralls --help");
            results.ShouldContain("Options:");
            results.ShouldContain("Options:");
            results.ShouldContain("What its for:");
        }

        private string RunCoveralls(string arguments)
        {
            var process = new Process();
            var startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = Path.Combine(GetCoverallsExePath(), CoverallsExe);
            startInfo.Arguments = arguments;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit(5000);
            var results = process.StandardOutput.ReadToEnd();
            return results;
        }

        private string GetCoverallsExePath()
        {
#if DEBUG
            var configuration = "Debug";
#else
            var configuration = "Release";
#endif
            return Path.Combine("..","..","..", "csmacnz.Coveralls", "bin", configuration);
        }
    }
}
