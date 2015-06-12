using System;
using Xunit;

namespace csmacnz.Coveralls.Tests
{
    public class CoverageFileBuilderTests
    {
        [Fact]
        public void CanConstructBuilder()
        {
            var builder = CreateFileBuilder();
            Assert.NotNull(builder);
        }

        [Fact]
        public void NewBuilderCanCreateCoverageFile()
        {
            var builder = CreateFileBuilder();
            var coverageFile = builder.CreateFile();
            Assert.NotNull(coverageFile);
        }

        public class WhenCreatingACoverageFromADefaultBuilder
        {
            private readonly CoverageFile _coverageFile;
            private const string Filename = @"C:\sourceFile.cs";

            public  WhenCreatingACoverageFromADefaultBuilder()
            {
                var builder = CreateFileBuilder(Filename);
                _coverageFile = builder.CreateFile();
            }

            [Fact]
            public void ThenFileNameIsSet()
            {
                Assert.Equal(Filename, _coverageFile.Name);
            }

            [Fact]
            public void ThenSourceDigestIsTheEmptyDigest()
            {
                Assert.Equal("D41D8CD98F00B204E9800998ECF8427E", _coverageFile.SourceDigest);
            }

            [Fact]
            public void ThenCoverageIsEmpty()
            {
                Assert.NotNull(_coverageFile.Coverage);
                Assert.Equal(1, _coverageFile.Coverage.Length);
                Assert.Null(_coverageFile.Coverage[0]);
            }
        }

        [Fact]
        public void NewBuilderWithNullFileThrowsArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() => new CoverageFileBuilder(null));
        }

        public static CoverageFileBuilder CreateFileBuilder(string filePath = @"C:\temp\file.cs")
        {
            return new CoverageFileBuilder(new FileCoverageData(filePath, new int?[1]));
        }
    }
}
