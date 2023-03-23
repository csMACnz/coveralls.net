﻿using System;
using BCLExtensions;
using Beefeater;
using csmacnz.Coveralls.Ports;

namespace csmacnz.Coveralls.Tests.TestAdapters;

public class TestCoverallsService : ICoverallsService
{
    private readonly bool _isWorking;

    public TestCoverallsService(bool isWorking) => _isWorking = isWorking;

    public Result<Unit, string> PushParallelCompleteWebhook(string repoToken, string? buildNumber, Uri serverUrl, string? carryForward) => _isWorking ? Success : "An Error In the Test Service";

    public Result<Unit, string> Upload(string fileData, Uri serverUrl) => _isWorking ? Success : "An Error In the Test Service";

    private static Result<Unit, string> Success => Result<Unit, string>.OfValue(Unit.Default);
}
