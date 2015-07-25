using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Xunit.Sdk;

namespace csmacnz.Coveralls.Tests.Integration
{
    public static class CoverallsTestRunner
    {
        private const string CoverallsExe = "csmacnz.Coveralls.exe";

        public static CoverageRunResults RunCoveralls(string arguments)
        {
            var exePath = Path.Combine(GetCoverallsExePath(), CoverallsExe);
            var argumentsToUse = arguments;
            var fileNameToUse = exePath;
            if (Environment.GetEnvironmentVariable("MONO_INTEGRATION_MODE") == "True")
            {
                fileNameToUse = GetMonoPath();

                argumentsToUse = exePath + " " + arguments;
            }

            var process = new Process();
            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = fileNameToUse,
                Arguments = argumentsToUse,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };
            process.StartInfo = startInfo;

            string results;
            string errorsResults;
            int exitCode;
            using (process)
            {
                process.Start();

                results = process.StandardOutput.ReadToEnd();
                errorsResults = process.StandardError.ReadToEnd();
                Console.WriteLine(results);

                const int timeoutInMilliseconds = 10000;
                if (!process.WaitForExit(timeoutInMilliseconds))
                {
                    throw new XunitException(string.Format("Test execution time exceeded: {0}ms", timeoutInMilliseconds));
                }
                exitCode = process.ExitCode;
            }

            return new CoverageRunResults
            {
                StandardOutput=results,
                StandardError = errorsResults,
                ExitCode=exitCode
            };
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

        private static string GetCoverallsExePath()
        {
#if DEBUG
            var configuration = "Debug";
#else
            var configuration = "Release";
#endif
            var basePath = Environment.GetEnvironmentVariable("COVERALLS_TEST_BASEPATH");
            if (string.IsNullOrWhiteSpace(basePath))
            {
                basePath = Path.Combine("..", "..", "..");
            }
            Console.WriteLine(basePath);
            return Path.Combine(basePath, "csmacnz.Coveralls", "bin", configuration);
        }
    }
}
