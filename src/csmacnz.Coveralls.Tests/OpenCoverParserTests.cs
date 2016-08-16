using System.Diagnostics;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using csmacnz.Coveralls.Parsers;
using NSubstitute;
using Xunit;

namespace csmacnz.Coveralls.Tests
{
    public class OpenCoverParserTests
    {
        private IFileSystem _fileSystem;
        private IParser _parser;

        [Fact]
        public void EmptyReportLoadsNoSourceFiles()
        {
            InitializeWithResource("csmacnz.Coveralls.Tests.EmptyReport.xml");

            var results = _parser.GenerateSourceFiles("csmacnz.Coveralls.Tests.EmptyReport.xml", true);

            Assert.Equal(0, results.Count);
        }

        [Fact]
        public void SingleFileReportLoadsSingleSourceFiles()
        {
            InitializeWithResource("csmacnz.Coveralls.Tests.SingleFileReport.xml");

            var results = _parser.GenerateSourceFiles("csmacnz.Coveralls.Tests.SingleFileReport.xml", false);

            Assert.Equal(1, results.Count);
        }

        [Fact]
        public void SingleFileReportWithSingleMethodLineCoveredWithoutSourceLoadsCorrectly()
        {
            InitializeWithResource("csmacnz.Coveralls.Tests.SingleFileReportOneLineCovered.xml");

            var results = _parser.GenerateSourceFiles("csmacnz.Coveralls.Tests.SingleFileReportOneLineCovered.xml", true);

            Assert.Equal(1, results[0].Coverage[8]);
        }

        [Fact]
        public void SingleFileReportWithSingleMethodLineUncoveredWithoutSourceLoadsCorrectly()
        {
            InitializeWithResource("csmacnz.Coveralls.Tests.SingleFileReportOneLineUncovered.xml");

            var results = _parser.GenerateSourceFiles("csmacnz.Coveralls.Tests.SingleFileReportOneLineUncovered.xml", false);

            Assert.Equal(0, results[0].Coverage[8]);
        }

        private void InitializeWithResource(string embeddedResource)
        {
            _fileSystem = Substitute.For<IFileSystem>();
            _parser = new OpenCoverParser(new PathProcessor(""), _fileSystem);
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
            _fileSystem.LoadDocument(Arg.Any<string>()).Returns(document);
        }

    }
}
