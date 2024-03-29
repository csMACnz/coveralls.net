﻿namespace csmacnz.Coveralls.Tests.TestHelpers;

public class CoverallsRunResults
{
    public CoverallsRunResults(
        string standardOutput,
        string standardError,
        int exitCode)
    {
        StandardOutput = standardOutput;
        StandardError = standardError;
        ExitCode = exitCode;
    }

    public string StandardOutput { get; }

    public string StandardError { get; }

    public int ExitCode { get; }
}
