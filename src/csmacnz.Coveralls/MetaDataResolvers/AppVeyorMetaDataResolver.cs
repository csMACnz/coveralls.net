namespace csmacnz.Coveralls.MetaDataResolvers;

public class AppVeyorMetaDataResolver : IMetaDataResolver
{
    private readonly IEnvironmentVariables _variables;

    public AppVeyorMetaDataResolver(IEnvironmentVariables variables) => _variables = variables;

    public bool IsActive() => _variables.GetBooleanVariable("APPVEYOR");

    public Option<string> ResolveServiceName() => "appveyor";

    public Option<string> ResolveServiceJobId() => GetFromVariable("APPVEYOR_JOB_ID");

    public Option<string> ResolveServiceBuildNumber() => GetFromVariable("APPVEYOR_BUILD_NUMBER");

    public Option<string> ResolvePullRequestId() => GetFromVariable("APPVEYOR_PULL_REQUEST_NUMBER");

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
