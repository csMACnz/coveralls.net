using System;
using csmacnz.CommandLineArgumentStringParser;
using csmacnz.Coveralls.Tests.TestAdapters;

namespace csmacnz.Coveralls.Tests.TestHelpers
{
    public static class CoverallsTestRunner
    {
        public static CoverallsRunResults RunCoveralls(string arguments, TestFileSystem testFileSystem = null)
        {
            var testConsole = new TestConsole();
            var exitCode = new Program(testConsole, testFileSystem ?? new TestFileSystem(), "1.0.0.0").Run(ArgsParser.Parse(arguments)) ?? 0;

            var results = string.Join("\n", testConsole.StandardOut);
            Console.WriteLine(results);

            return new CoverallsRunResults
            {
                StandardError = string.Join("\n", testConsole.Errors),
                StandardOutput = results,
                ExitCode = exitCode
            };
        }
    }
}
