using System;
using System.Linq;
using csmacnz.Coveralls.Parsers;
using Xunit;

namespace csmacnz.Coveralls.Tests
{
    public class ChutzpahJsonParserTests
    {
        [Fact]
        public void GenerateSourceFiles_CorrectCoverage()
        {
            var fileContents = Reports.ChutzpahExample.Split(new[] {Environment.NewLine}, StringSplitOptions.None);

            var results = ChutzpahJsonParser.GenerateSourceFiles(fileContents);

            Assert.Equal(2, results.Count);
            Assert.Equal(@"D:\path\to\file\file.ts", results.First().FullPath);
            Assert.Equal(36, results.First().Coverage[0]);
            Assert.Equal(10, results.First().Coverage[5]);
            Assert.Null(results.First().Coverage[7]);
        }
    }
}
