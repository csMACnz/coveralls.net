﻿namespace csmacnz.Coveralls.MetaDataResolvers;

public class TravisMetaDataResolver : IMetaDataResolver
{
    private readonly IEnvironmentVariables _variables;

    public TravisMetaDataResolver(IEnvironmentVariables variables) => _variables = variables;

    public bool IsActive() => _variables.GetBooleanVariable("TRAVIS");

    public Option<string> ResolveServiceName() => "travis";

    public Option<string> ResolveServiceJobId() => GetFromVariable("TRAVIS_JOB_ID");

    public Option<string> ResolveServiceBuildNumber() => GetFromVariable("TRAVIS_BUILD_NUMBER");

    public Option<string> ResolvePullRequestId()
    {
        var value = GetFromVariable("TRAVIS_PULL_REQUEST");
        return value.Match(
            val => val == "false" ? Option<string>.None : val,
            () => Option<string>.None);
    }

    private Option<string> GetFromVariable(string variableName)
    {
        var prId = _variables.GetEnvironmentVariable(variableName);

        if (prId.IsNotNullOrWhitespace())
        {
            return prId;
        }

        return Option<string>.None;
    }
}
