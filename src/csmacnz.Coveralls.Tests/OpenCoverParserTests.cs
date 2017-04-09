using System.Diagnostics;
using System.Reflection;
using System.Xml;
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
            var document = LoadDocumentFromResource("csmacnz.Coveralls.Tests.EmptyReport.xml");

            var results = CreateOpenCoverParser().GenerateSourceFiles(document);

            Assert.Equal(0, results.Count);
        }

        [Fact]
        public void SingleFileReportLoadsSingleSourceFiles()
        {
            var document = LoadDocumentFromResource("csmacnz.Coveralls.Tests.SingleFileReport.xml");

            var results = CreateOpenCoverParser().GenerateSourceFiles(document);

            Assert.Equal(1, results.Count);
        }

        [Fact]
        public void SingleFileReportWithSingleMethodLineCoveredWithoutSourceLoadsCorrectly()
        {
            var document = LoadDocumentFromResource("csmacnz.Coveralls.Tests.SingleFileReportOneLineCovered.xml");

            var results = CreateOpenCoverParser().GenerateSourceFiles(document);

            Assert.Equal(1, results[0].Coverage[8]);
        }

        [Fact]
        public void SingleFileReportWithSingleMethodLineUncoveredWithoutSourceLoadsCorrectly()
        {
            var document = LoadDocumentFromResource("csmacnz.Coveralls.Tests.SingleFileReportOneLineUncovered.xml");

            var results = CreateOpenCoverParser().GenerateSourceFiles(document);

            Assert.Equal(0, results[0].Coverage[8]);
        }

        private OpenCoverParser CreateOpenCoverParser()
        {
            return new OpenCoverParser();
        }

        private static XDocument LoadDocumentFromResource(string embeddedResource)
        {
            XDocument document;
            var executingAssembly = Assembly.GetExecutingAssembly();
            using (var stream = executingAssembly.GetManifestResourceStream(embeddedResource))
            {
                Assert.NotNull(stream);
                Debug.Assert(stream != null, "stream != null");
                using (var reader = XmlReader.Create(stream))
                {
                    document = XDocument.Load(reader);
                }
            }
            return document;
        }
    }
}