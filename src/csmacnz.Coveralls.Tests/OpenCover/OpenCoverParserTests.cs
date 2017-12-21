using System.Xml.Linq;
using csmacnz.Coveralls.Parsers;
using Xunit;

namespace csmacnz.Coveralls.Tests.OpenCover
{
    public class OpenCoverParserTests
    {
        [Fact]
        public void EmptyReportLoadsNoSourceFiles()
        {
            var document = XDocument.Parse(Reports.OpenCoverSamples.EmptyReport);

            var results = OpenCoverParser.GenerateSourceFiles(document);

            Assert.Empty(results);
        }

        [Fact]
        public void SingleFileReportLoadsSingleSourceFiles()
        {
            var document = XDocument.Parse(Reports.OpenCoverSamples.SingleFileReport);

            var results = OpenCoverParser.GenerateSourceFiles(document);

            Assert.Single(results);
        }

        [Fact]
        public void SingleFileReportWithSingleMethodLineCoveredWithoutSourceLoadsCorrectly()
        {
            var document = XDocument.Parse(Reports.OpenCoverSamples.SingleFileReportOneLineCovered);

            var results = OpenCoverParser.GenerateSourceFiles(document);

            Assert.Equal(1, results[0].Coverage[8]);
        }

        [Fact]
        public void SingleFileReportWithSingleMethodLineUncoveredWithoutSourceLoadsCorrectly()
        {
            var document = XDocument.Parse(Reports.OpenCoverSamples.SingleFileReportOneLineUncovered);

            var results = OpenCoverParser.GenerateSourceFiles(document);

            Assert.Equal(0, results[0].Coverage[8]);
        }
    }
}
