using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace csmacnz.Coveralls.Tests
{
    [TestClass]
    public class OpenCoverParserTests
    {
        private const string SingleFileReportSourceFilePath = @"c:\Users\Mark\Documents\Visual Studio 2013\Projects\OpenCoverTesting\OpenCoverTesting\Class1.cs";

        [TestMethod]
        public void EmptyReportLoadsNoSourceFiles()
        {
            var document = LoadDocumentFromResource("csmacnz.Coveralls.Tests.EmptyReport.xml");

            var results = CreateOpenCoverParser().GenerateSourceFiles(document);
            
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void SingleFileReportLoadsSingleSourceFiles()
        {
            var document = LoadDocumentFromResource("csmacnz.Coveralls.Tests.SingleFileReport.xml");

            var results = CreateOpenCoverParser().GenerateSourceFiles(document);
            
            Assert.AreEqual(1, results.Count);
        }

        [TestMethod]
        public void SingleFileReportWithSingleMethodLineCoveredWithoutSourceLoadsCorrectly()
        {
            var document = LoadDocumentFromResource("csmacnz.Coveralls.Tests.SingleFileReportOneLineCovered.xml");

            var results = CreateOpenCoverParserForSingleFileReport().GenerateSourceFiles(document);
            
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(SingleFileReportSourceFilePath, results[0].Name);
            Assert.AreEqual(12, results[0].Coverage.Length);
            Assert.AreEqual(1, results[0].Coverage[8]);
        }

        [TestMethod]
        public void SingleFileReportWithSingleMethodLineUncoveredWithoutSourceLoadsCorrectly()
        {
            var document = LoadDocumentFromResource("csmacnz.Coveralls.Tests.SingleFileReportOneLineUncovered.xml");

            var results = CreateOpenCoverParserForSingleFileReport().GenerateSourceFiles(document);
            
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(SingleFileReportSourceFilePath, results[0].Name);
            Assert.AreEqual(12, results[0].Coverage.Length);
            Assert.AreEqual(0, results[0].Coverage[8]);
        }

        private OpenCoverParser CreateOpenCoverParser()
        {
            return new OpenCoverParser(new TestFileSystem());
        }

        private OpenCoverParser CreateOpenCoverParserForSingleFileReport()
        {
            var testFileSystem = new TestFileSystem();
            var singleFileReportSourceContent = LoadContentFromResource("csmacnz.Coveralls.Tests.SingleFileReportSourceFile.txt");
            testFileSystem.AddFile(SingleFileReportSourceFilePath, singleFileReportSourceContent);
            return new OpenCoverParser(testFileSystem);
        }

        private static XDocument LoadDocumentFromResource(string embeddedResource)
        {
            XDocument document;
            var executingAssembly = Assembly.GetExecutingAssembly();
            using (var stream = executingAssembly.GetManifestResourceStream(embeddedResource))
            {
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
                using (var reader = new StreamReader(stream))
                {
                    content = reader.ReadToEnd();
                }
            }
            return content;
        }
    }
}
