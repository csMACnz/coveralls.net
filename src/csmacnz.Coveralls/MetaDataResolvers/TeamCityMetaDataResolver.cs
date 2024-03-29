﻿namespace csmacnz.Coveralls.MetaDataResolvers;

public class TeamCityMetaDataResolver : IMetaDataResolver
{
    private readonly IEnvironmentVariables _variables;

    public TeamCityMetaDataResolver(IEnvironmentVariables variables) => _variables = variables;

    public bool IsActive() => _variables.GetEnvironmentVariable("TEAMCITY_VERSION").IsNotNullOrWhitespace();

    public Option<string> ResolveServiceName() => "teamcity";

    public Option<string> ResolveServiceJobId() => Option<string>.None;

    public Option<string> ResolveServiceBuildNumber() =>

        // %build.counter%
        GetFromVariable("TEAMCITY_BUILD_NUMBER");

    public Option<string> ResolvePullRequestId()
    {
        var value = GetFromVariable("TEAMCITY_PULL_REQUEST");

        // Edge case: regex for `refs/pull/86/merge` && `refs/pull/86/head` needed
        return value.Match(
            val => int.TryParse(val.Trim(), out var _) ? val : Option<string>.None,
            () => Option<string>.None);
    }

    private Option<string> GetFromVariable(string variableName)
    {
        var prId = _variables.GetEnvironmentVariable(variableName);

        if (prId.IsNotNullOrWhitespace())
        {
            return new Option<string>(prId);
        }

        return Option<string>.None;
    }
}
