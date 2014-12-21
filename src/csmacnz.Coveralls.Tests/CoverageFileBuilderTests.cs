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
            public void ThenSourceIsEmpty()
            {
                Assert.Equal(string.Empty, _coverageFile.Source);
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
        public void NewBuilderWithInvalidPathThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new CoverageFileBuilder(""));
        }

        public static CoverageFileBuilder CreateFileBuilder(string filePath = @"C:\temp\file.cs")
        {
            return new CoverageFileBuilder(filePath);
        }
    }
}
