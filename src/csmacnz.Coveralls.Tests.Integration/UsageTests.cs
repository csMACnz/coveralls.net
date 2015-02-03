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
            var exePath = Path.Combine(GetCoverallsExePath(), CoverallsExe);
            var argumentsToUse = arguments;
            var fileNameToUse = exePath;
            if (Environment.GetEnvironmentVariable("MONO_INTEGRATION_MODE") == "WINDOWS")
            {
                fileNameToUse = GetMonoPath();

                argumentsToUse = exePath + " " + arguments;
            }
            if (Environment.GetEnvironmentVariable("MONO_INTEGRATION_MODE") == "SH")
            {
                fileNameToUse = "sh";

                var mono = GetMonoPath();

                argumentsToUse = "-C \"" + mono + " " + exePath + " " + arguments + "\"";
            }

            var process = new Process();
            var startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = fileNameToUse;
            startInfo.Arguments = argumentsToUse;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit(5000);
            var results = process.StandardOutput.ReadToEnd();
            return results;
        }

        private static string GetMonoPath()
        {
            var monoApp = "mono";
            var monoPath = Environment.GetEnvironmentVariable("MONO_INTEGRATION_MONOPATH");
            if (!string.IsNullOrWhiteSpace(monoPath))
            {
                monoApp = monoPath + Path.DirectorySeparatorChar + "mono";
            }
            return monoApp;
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
