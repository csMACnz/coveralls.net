using System.Xml.Linq;
using csmacnz.Coveralls.Parsers;
using Xunit;

namespace csmacnz.Coveralls.Tests.NCover;

public class NCoverParserTests
{
    [Fact]
    public void EmptyReportLoadsNoSourceFiles()
    {
        var document = XDocument.Parse(Reports.NCoverSamples.EmptyReport);

        var results = NCoverParser.GenerateSourceFiles(document);

        Assert.Empty(results);
    }

    [Fact]
    public void SingleFileReportLoadsSingleSourceFiles()
    {
        var document = XDocument.Parse(Reports.NCoverSamples.SingleFileReportOneLineCovered.Report);

        var results = NCoverParser.GenerateSourceFiles(document);

        Assert.Single(results);
    }

    [Fact]
    public void SingleFileReportWithSingleMethodLineCoveredWithoutSourceLoadsCorrectly()
    {
        var document = XDocument.Parse(Reports.NCoverSamples.SingleFileReportOneLineCovered.Report);

        var results = NCoverParser.GenerateSourceFiles(document);

        Assert.Equal(1, results[0].Coverage[11]);
    }

    [Fact]
    public void SingleFileReportWithSingleMethodLineUncoveredWithoutSourceLoadsCorrectly()
    {
        var document = XDocument.Parse(Reports.NCoverSamples.SingleFileReportOneLineUncovered);

        var results = NCoverParser.GenerateSourceFiles(document);

        Assert.Equal(0, results[0].Coverage[11]);
    }
}
