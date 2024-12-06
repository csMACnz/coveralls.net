using System;
using csmacnz.Coveralls.Parsers;
using Xunit;

namespace csmacnz.Coveralls.Tests.Chutzpah;

public class ChutzpahJsonParserTests
{
    [Fact]
    public void GenerateSourceFiles_CorrectCoverage()
    {
        var fileContents = Reports.ChutzpahSample.ChutzpahExample.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

        var results = ChutzpahJsonParser.GenerateSourceFiles(fileContents);
        var first = results[0];
        Assert.Equal(2, results.Count);
        Assert.Equal(@"D:\path\to\file\file.ts", first.FullPath);
        Assert.Equal(36, first.Coverage[0]);
        Assert.Equal(10, first.Coverage[5]);
        Assert.Null(first.Coverage[7]);
    }
}
