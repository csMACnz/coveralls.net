using System;
using System.Collections.Generic;
using Xunit;

namespace csmacnz.Coveralls.Tests
{
    public class AppVeyorGitDataResolverTests
    {
        [Fact]
        public void CanProvideDataNoEnvironmentVariablesSetReturnsFalse()
        {
            var sut = new AppVeyorGitDataResolver(new TestEnvironmentVariables(new Dictionary<string, string>()));

            var canProvideData = sut.CanProvideData();

            Assert.False(canProvideData);
        }

        [Fact]
        public void CanProvideDataAppVeyorEnvironmentVariableSetToFalseReturnsFalse()
        {
            IEnvironmentVariables variables = new TestEnvironmentVariables(new Dictionary<string, string>
            {
                {"APPVEYOR", "False"}
            });

            var sut = new AppVeyorGitDataResolver(variables);

            var canProvideData = sut.CanProvideData();

            Assert.False(canProvideData);
        }

        [Fact]
        public void CanProvideDataAppVeyorEnvironmentVariablesSetReturnsTrue()
        {
            IEnvironmentVariables variables = new TestEnvironmentVariables(new Dictionary<string, string>
            {
                {"APPVEYOR", "True"}
            });

            var sut = new AppVeyorGitDataResolver(variables);

            var canProvideData = sut.CanProvideData();

            Assert.True(canProvideData);
        }

        [Fact]
        public void GenerateDataNoEnviromentDataReturnsEmptyGitData()
        {
            IEnvironmentVariables variables = new TestEnvironmentVariables(new Dictionary<string, string>
            {
                {"APPVEYOR", "True"}
            });

            var sut = new AppVeyorGitDataResolver(variables);

            var gitData = sut.GenerateData();

            Assert.NotNull(gitData);
        }
        public class GenerateData
        {
            private readonly GitData _gitData;
            private readonly string _expectedId;
            private readonly string _expectedName;
            private readonly string _expectedEmail;
            private readonly string _expectedMessage;
            private readonly string _expectedBranch;

            public GenerateData()
            {
                _expectedId = Guid.NewGuid().ToString();
                _expectedName = "Test User Name";
                _expectedEmail = "email@example.com";
                _expectedMessage = "Add a new widget\n* some code\n* some tests";
                _expectedBranch = "feature";

                IEnvironmentVariables variables = new TestEnvironmentVariables(new Dictionary<string, string>
                {
                    {"APPVEYOR", "True"},
                    {"APPVEYOR_REPO_COMMIT", _expectedId},
                    {"APPVEYOR_REPO_COMMIT_AUTHOR", _expectedName},
                    {"APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL", _expectedEmail},
                    {"APPVEYOR_REPO_COMMIT_MESSAGE", _expectedMessage},
                    {"APPVEYOR_REPO_BRANCH", _expectedBranch}
                });

                var sut = new AppVeyorGitDataResolver(variables);

                _gitData = sut.GenerateData();
            }

            [Fact]
            public void CommitIdSetCorrectly()
            {
                Assert.NotNull(_gitData.Head);
                Assert.Equal(_expectedId, _gitData.Head.Id);
            }

            [Fact]
            public void NameSetCorrectly()
            {
                Assert.NotNull(_gitData.Head);
                Assert.Equal(_expectedName, _gitData.Head.AuthorName);
                Assert.Equal(_expectedName, _gitData.Head.CommitterName);
            }

            [Fact]
            public void EmailSetCorrectly()
            {
                Assert.NotNull(_gitData.Head);
                Assert.Equal(_expectedEmail, _gitData.Head.AuthorEmail);
                Assert.Equal(_expectedEmail, _gitData.Head.ComitterEmail);
            }


            [Fact]
            public void MessageSetCorrectly()
            {
                Assert.NotNull(_gitData.Head);
                Assert.Equal(_expectedMessage, _gitData.Head.Message);
            }


            [Fact]
            public void BranchSetCorrectly()
            {
                Assert.Equal(_expectedBranch, _gitData.Branch);
            }
        }
    }

    public class TestEnvironmentVariables : IEnvironmentVariables
    {
        private Dictionary<string, string> _variables;

        public TestEnvironmentVariables(Dictionary<string, string> variables)
        {
            _variables = variables;
        }

        public string GetEnvironmentVariable(string key)
        {
            return _variables.ContainsKey(key) ? _variables[key] : string.Empty;
        }
    }
}
