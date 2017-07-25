using System.Xml.Linq;
using csmacnz.Coveralls.Parsers;
using Xunit;

namespace csmacnz.Coveralls.Tests
{
    public class OpenCoverParserTests
    {
        [Fact]
        public void EmptyReportLoadsNoSourceFiles()
        {
            var document = XDocument.Parse(Reports.EmptyReport);

            var results = OpenCoverParser.GenerateSourceFiles(document);

            Assert.Equal(0, results.Count);
        }

        [Fact]
        public void SingleFileReportLoadsSingleSourceFiles()
        {
            var document = XDocument.Parse(Reports.SingleFileReport);

            var results = OpenCoverParser.GenerateSourceFiles(document);

            Assert.Equal(1, results.Count);
        }

        [Fact]
        public void SingleFileReportWithSingleMethodLineCoveredWithoutSourceLoadsCorrectly()
        {
            var document = XDocument.Parse(Reports.SingleFileReportOneLineCovered);

            var results = OpenCoverParser.GenerateSourceFiles(document);

            Assert.Equal(1, results[0].Coverage[8]);
        }

        [Fact]
        public void SingleFileReportWithSingleMethodLineUncoveredWithoutSourceLoadsCorrectly()
        {
            var document = XDocument.Parse(Reports.SingleFileReportOneLineUncovered);

            var results = OpenCoverParser.GenerateSourceFiles(document);

            Assert.Equal(0, results[0].Coverage[8]);
        }
    }
}