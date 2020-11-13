using System;
using csMACnz.Caps;
using csmacnz.Coveralls.Tests.TestAdapters;

namespace csmacnz.Coveralls.Tests.TestHelpers
{
    public static class CoverallsTestRunner
    {
        public static CoverallsRunResults RunCoveralls(string arguments, TestFileSystem? testFileSystem = null, TestEnvironmentVariables? testEnvironmentVariables = null)
        {
            var testConsole = new TestConsole();
            var exitCode = new Program(
                testConsole,
                testFileSystem ?? new TestFileSystem(),
                testEnvironmentVariables ?? new TestEnvironmentVariables(),
                new TestCoverallsService(isWorking: true),
                "1.0.0.0")
                .Run(ArgsParser.Parse(arguments)) ?? 0;

            var results = string.Join("\n", testConsole.StandardOut);
            Console.WriteLine(results);

            return new CoverallsRunResults(
                standardOutput: results,
                standardError: string.Join("\n", testConsole.Errors),
                exitCode: exitCode);
        }
    }
}
