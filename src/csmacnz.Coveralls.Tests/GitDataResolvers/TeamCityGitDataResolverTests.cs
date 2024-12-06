using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using csmacnz.Coveralls.GitDataResolvers;
using csmacnz.Coveralls.Ports;
using csmacnz.Coveralls.Tests.TestAdapters;
using Xunit;

namespace csmacnz.Coveralls.Tests.GitDataResolvers;

public class TeamCityGitDataResolverTests
{
    [Fact]
    public void CanProvideDataNoEnvironmentVariablesSetReturnsFalse()
    {
        var sut = new TeamCityGitDataResolver(new TestEnvironmentVariables(), new TestConsole());

        var canProvideData = sut.CanProvideData();

        Assert.False(canProvideData);
    }

    [Fact]
    public void CanProvideDataTeamCityEnvironmentVariableSetReturnsTrue()
    {
        IEnvironmentVariables variables = new TestEnvironmentVariables(new Dictionary<string, string>
        {
            { "TEAMCITY_VERSION", "10.4.5-monsoon" }
        });

        var sut = new TeamCityGitDataResolver(variables, new TestConsole());

        var canProvideData = sut.CanProvideData();

        Assert.True(canProvideData);
    }

    [Fact]
    public void GenerateDataNoCustomEnviromentDataReturnsCommitSha()
    {
        var sha = "46d8bffca535dd350b0167d0eb58a22d4bf4ea6e";
        IEnvironmentVariables variables = new TestEnvironmentVariables(new Dictionary<string, string>
        {
            { "TEAMCITY_VERSION", "10.4.5-monsoon" },
            { "BUILD_VCS_NUMBER", sha }
        });

        var sut = new TeamCityGitDataResolver(variables, new TestConsole());

        var gitData = sut.GenerateData();

        AssertNotNull(gitData!);
        Assert.True(gitData.Value.IsItem2);
        Assert.Equal(sha, gitData.Value.Item2.Value);
    }

    [Fact]
    public void GenerateDataCustomEnviromentDataReturnsGitData()
    {
        var sha = "46d8bffca535dd350b0167d0eb58a22d4bf4ea6e";
        var branch = "master";

        IEnvironmentVariables variables = new TestEnvironmentVariables(new Dictionary<string, string>
        {
            { "TEAMCITY_VERSION", "10.4.5-monsoon" },
            { "TEAMCITY_BUILD_BRANCH", branch },
            { "TEAMCITY_BUILD_COMMIT", sha }
        });

        var sut = new TeamCityGitDataResolver(variables, new TestConsole());

        var gitData = sut.GenerateData();

        AssertNotNull(gitData!);
        Assert.True(gitData.Value.IsItem1);
        Assert.Equal(branch, gitData.Value.Item1.Branch);
        AssertNotNull(gitData.Value.Item1.Head!);
        Assert.Equal(sha, gitData.Value.Item1.Head.Id);
    }

    private static void AssertNotNull([NotNull] object t) => Assert.NotNull(t);
}
