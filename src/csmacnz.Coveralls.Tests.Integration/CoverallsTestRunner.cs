using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Xunit.Sdk;

namespace csmacnz.Coveralls.Tests.Integration
{
    public static class CoverallsTestRunner
    {
        public static CoverallsRunResults RunCoveralls(string arguments)
        {
            var applicationProcess = "dotnet";
            var applicationPath = GetCoverallsDll();
            var argumentsToUse = "exec " + applicationPath + " " + arguments;

            var exePath = Environment.GetEnvironmentVariable("COVERALLSNET_EXEPATH");
            if (!string.IsNullOrWhiteSpace(exePath))
            {
                applicationProcess = exePath;
                argumentsToUse = arguments;
            }

            string results;
            string errorsResults;
            int exitCode;
            using (var process = new Process())
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = applicationProcess,
                    Arguments = argumentsToUse,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                };
                process.StartInfo = startInfo;

                process.Start();

                results = process.StandardOutput.ReadToEnd();
                errorsResults = process.StandardError.ReadToEnd();
                Console.WriteLine(results);

                const int timeoutInMilliseconds = 10000;
                if (!process.WaitForExit(timeoutInMilliseconds))
                {
                    throw new XunitException($"Test execution time exceeded: {timeoutInMilliseconds}ms");
                }

                exitCode = process.ExitCode;
            }

            return new CoverallsRunResults
            {
                StandardOutput = results,
                StandardError = errorsResults,
                ExitCode = exitCode
            };
        }

        private static string GetCoverallsDll()
        {
#if DEBUG
            var configuration = "Debug";
#else
            var configuration = "Release";
#endif
            return Path.Combine("..", "..", "..", "..", "csmacnz.Coveralls", "bin", configuration, "netcoreapp1.1", "csmacnz.Coveralls.dll");
        }
    }
}
