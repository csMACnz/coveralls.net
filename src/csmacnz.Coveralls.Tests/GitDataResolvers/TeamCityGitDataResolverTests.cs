using System;
using System.Collections.Generic;
using Beefeater;
using csmacnz.Coveralls.Data;
using csmacnz.Coveralls.GitDataResolvers;
using csmacnz.Coveralls.Ports;
using csmacnz.Coveralls.Tests.TestAdapters;
using Xunit;

namespace csmacnz.Coveralls.Tests.GitDataResolvers
{
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
            string sha = "46d8bffca535dd350b0167d0eb58a22d4bf4ea6e";
            IEnvironmentVariables variables = new TestEnvironmentVariables(new Dictionary<string, string>
            {
                { "TEAMCITY_VERSION", "10.4.5-monsoon" },
                { "BUILD_VCS_NUMBER", sha }
            });

            var sut = new TeamCityGitDataResolver(variables, new TestConsole());

            var gitData = sut.GenerateData();

            Assert.True(gitData.HasValue);
            Assert.True(gitData.IsItem2);
            Assert.Equal(sha, gitData.Item2.Value);
        }

        [Fact]
        public void GenerateDataCustomEnviromentDataReturnsGitData()
        {
            string sha = "46d8bffca535dd350b0167d0eb58a22d4bf4ea6e";
            string branch = "master";

            IEnvironmentVariables variables = new TestEnvironmentVariables(new Dictionary<string, string>
            {
                { "TEAMCITY_VERSION", "10.4.5-monsoon" },
                { "TEAMCITY_BUILD_BRANCH", branch },
                { "TEAMCITY_BUILD_COMMIT", sha }
            });

            var sut = new TeamCityGitDataResolver(variables, new TestConsole());

            var gitData = sut.GenerateData();

            Assert.True(gitData.HasValue);
            Assert.True(gitData.IsItem1);
            Assert.Equal(branch, gitData.Item1.Branch);
            Assert.NotNull(gitData.Item1.Head);
            Assert.Equal(sha, gitData.Item1.Head.Id);
        }
    }
}
