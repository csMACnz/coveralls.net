using System.Collections.Generic;
using csmacnz.Coveralls.MetaDataResolvers;
using csmacnz.Coveralls.Ports;
using csmacnz.Coveralls.Tests.TestAdapters;
using Xunit;

namespace csmacnz.Coveralls.Tests.MetaDataResolvers
{
    public class TeamCityMetaDataResolverTests
    {
        [Fact]
        public void CanProvideDataNoEnvironmentVariablesSetReturnsFalse()
        {
            var sut = new TeamCityMetaDataResolver(new TestEnvironmentVariables());

            var canProvideData = sut.IsActive();

            Assert.False(canProvideData);
        }

        [Fact]
        public void CanProvideDataTeamCityEnvironmentVariableSetReturnsTrue()
        {
            IEnvironmentVariables variables = new TestEnvironmentVariables(new Dictionary<string, string>
            {
                { "TEAMCITY_VERSION", "10.4.5-monsoon" }
            });

            var sut = new TeamCityMetaDataResolver(variables);

            var canProvideData = sut.IsActive();

            Assert.True(canProvideData);
        }

        [Fact]
        public void TeamcityNoCustomEnvironmentVariableSetReturnsCorrectResults()
        {
            IEnvironmentVariables variables = new TestEnvironmentVariables(new Dictionary<string, string>
            {
                { "TEAMCITY_VERSION", "10.4.5-monsoon" }
            });

            var sut = new TeamCityMetaDataResolver(variables);

            var canProvideData = sut.IsActive();

            Assert.True(canProvideData);
            var serviceNameResult = sut.ResolveServiceName();
            Assert.True(serviceNameResult.HasValue);
            Assert.Equal("teamcity", serviceNameResult.ValueOrDefault());
            Assert.False(sut.ResolvePullRequestId().HasValue);
            Assert.False(sut.ResolveServiceBuildNumber().HasValue);
            Assert.False(sut.ResolveServiceJobId().HasValue);
        }

        public class GenerateData
        {
            private readonly string _expectedPullRequestId;
            private readonly string _expectedBuildNumber;
            private readonly string _expectedServiceName;
            private readonly TeamCityMetaDataResolver _sut;

            public GenerateData()
            {
                _expectedPullRequestId = "42";
                _expectedBuildNumber = "12";
                _expectedServiceName = "teamcity";

                IEnvironmentVariables variables = new TestEnvironmentVariables(new Dictionary<string, string>
                {
                    { "TEAMCITY_VERSION", "10.4.5-monsoon" },
                    { "TEAMCITY_BUILD_NUMBER", _expectedBuildNumber },
                    { "TEAMCITY_PULL_REQUEST", _expectedPullRequestId }
                });

                _sut = new TeamCityMetaDataResolver(variables);
            }

            [Fact]
            public void PullRequestIdSetCorrectly()
            {
                var result = _sut.ResolvePullRequestId();
                Assert.True(result.HasValue);
                Assert.Equal(_expectedPullRequestId, result.ValueOrDefault());
            }

            [Fact]
            public void ServiceBuildNumberResolvesCorrectly()
            {
                var result = _sut.ResolveServiceBuildNumber();
                Assert.True(result.HasValue);
                Assert.Equal(_expectedBuildNumber, result.ValueOrDefault());
            }

            [Fact]
            public void ServiceJobIdResolvesCorrectly()
            {
                var result = _sut.ResolveServiceJobId();
                Assert.False(result.HasValue);
            }

            [Fact]
            public void ServiceNameResolvesCorrectly()
            {
                var result = _sut.ResolveServiceName();
                Assert.True(result.HasValue);
                Assert.Equal(_expectedServiceName, result.ValueOrDefault());
            }
        }
    }
}
