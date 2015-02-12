using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using Xunit;

namespace csmacnz.Coveralls.Tests
{
    public class OpenCoverParserTests
    {
        private const string SingleFileReportSourceFilePath = @"c:\Users\Mark\Documents\Visual Studio 2013\Projects\OpenCoverTesting\OpenCoverTesting\Class1.cs";
        private const string ExpectedSingleFileReportSourceFilePath = @"c:/Users/Mark/Documents/Visual Studio 2013/Projects/OpenCoverTesting/OpenCoverTesting/Class1.cs";

        [Fact]
        public void EmptyReportLoadsNoSourceFiles()
        {
            var document = LoadDocumentFromResource("csmacnz.Coveralls.Tests.EmptyReport.xml");

            var results = CreateOpenCoverParser().GenerateSourceFiles(document, false);

            Assert.Equal(0, results.Count);
        }

        [Fact]
        public void SingleFileReportLoadsSingleSourceFiles()
        {
            var document = LoadDocumentFromResource("csmacnz.Coveralls.Tests.SingleFileReport.xml");

            var results = CreateOpenCoverParser().GenerateSourceFiles(document, false);

            Assert.Equal(1, results.Count);
        }

        [Fact]
        public void SingleFileReportWithSingleMethodLineCoveredWithoutSourceLoadsCorrectly()
        {
            var document = LoadDocumentFromResource("csmacnz.Coveralls.Tests.SingleFileReportOneLineCovered.xml");

            var results = CreateOpenCoverParserForSingleFileReport().GenerateSourceFiles(document, false);

            AssertSingleFileResult(ExpectedSingleFileReportSourceFilePath, 12, results);
            Assert.Equal(1, results[0].Coverage[8]);
        }

        [Fact]
        public void SingleFileReportWithSingleMethodLineUncoveredWithoutSourceLoadsCorrectly()
        {
            var document = LoadDocumentFromResource("csmacnz.Coveralls.Tests.SingleFileReportOneLineUncovered.xml");

            var results = CreateOpenCoverParserForSingleFileReport().GenerateSourceFiles(document, false);

            AssertSingleFileResult(ExpectedSingleFileReportSourceFilePath, 12, results);
            Assert.Equal(0, results[0].Coverage[8]);
        }

        private OpenCoverParser CreateOpenCoverParser()
        {
            return new OpenCoverParser(new TestFileSystem(), new PathProcessor());
        }

        private OpenCoverParser CreateOpenCoverParserForSingleFileReport()
        {
            var testFileSystem = new TestFileSystem();
            var singleFileReportSourceContent = LoadContentFromResource("csmacnz.Coveralls.Tests.SingleFileReportSourceFile.txt");
            testFileSystem.AddFile(SingleFileReportSourceFilePath, singleFileReportSourceContent);
            return new OpenCoverParser(testFileSystem,new PathProcessor());
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

        private static string LoadContentFromResource(string embeddedResource)
        {
            string content;
            var executingAssembly = Assembly.GetExecutingAssembly();
            using (var stream = executingAssembly.GetManifestResourceStream(embeddedResource))
            {
                Assert.NotNull(stream);
                Debug.Assert(stream != null, "stream != null");
                using (var reader = new StreamReader(stream))
                {
                    content = reader.ReadToEnd();
                }
            }
            return content;
        }

        // ReSharper disable UnusedParameter.Local
        private void AssertSingleFileResult(string expectedFileName, int expectedLength, List<CoverageFile> results)
        // ReSharper restore UnusedParameter.Local
        {
            Assert.Equal(1, results.Count);
            Assert.Equal(expectedFileName, results[0].Name);
            Assert.Equal(expectedLength, results[0].Coverage.Length);
        }
    }
}
