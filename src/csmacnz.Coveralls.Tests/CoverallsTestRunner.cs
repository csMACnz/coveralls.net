using System;
using System.Collections.Generic;
using csmacnz.CLIArgsParser;

namespace csmacnz.Coveralls.Tests
{
    public static class CoverallsTestRunner
    {
        private const string CoverallsExe = "csmacnz.Coveralls.exe";

        public static CoverallsRunResults RunCoveralls(string arguments)
        {
            var testConsole = new TestConsole();
            var exitCode = new Program(testConsole, "1.0.0.0").Run(ArgsParser.Parse(arguments)) ?? 0;

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

    public class TestConsole: IConsole
    {
        private readonly List<string> _errors = new List<string>();
        private readonly List<string> _standardOut = new List<string>();

        public void WriteLine(string message)
        {
            _standardOut.Add(message);
        }

        public void WriteErrorLine(string message)
        {
            _errors.Add(message);
        }
        public string[] Errors => _errors.ToArray();
        public string[] StandardOut => _standardOut.ToArray();
    }
}