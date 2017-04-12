using System.Diagnostics;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using csmacnz.Coveralls.Parsers;
using Xunit;
using System.Collections.Generic;

namespace csmacnz.Coveralls.Tests
{
    public class ReportGeneratorParserTests
    {
        [Fact]
        public void EmptyReportLoadsNoSourceFiles()
        {
            var documents = new Dictionary<string, XDocument>
            {
                ["C:\\test\\GameOfLife.xUnit.Tests_GameOfLife.xUnit.Tests.WorldTests.xml"] = LoadDocumentFromResource("csmacnz.Coveralls.Tests.ReportGeneratorSample.GameOfLife.xUnit.Tests_GameOfLife.xUnit.Tests.WorldTests.xml"),
                ["C:\\test\\GameOfLife_GameOfLife.Game.xml"] = LoadDocumentFromResource("csmacnz.Coveralls.Tests.ReportGeneratorSample.GameOfLife_GameOfLife.Game.xml"),
                ["C:\\test\\GameOfLife_GameOfLife.Program.xml"] = LoadDocumentFromResource("csmacnz.Coveralls.Tests.ReportGeneratorSample.GameOfLife_GameOfLife.Program.xml"),
                ["C:\\test\\GameOfLife_GameOfLife.World.xml"] = LoadDocumentFromResource("csmacnz.Coveralls.Tests.ReportGeneratorSample.GameOfLife_GameOfLife.World.xml"),
                ["C:\\test\\GameOfLife_GameOfLife.WorldBuilder.xml"] = LoadDocumentFromResource("csmacnz.Coveralls.Tests.ReportGeneratorSample.GameOfLife_GameOfLife.WorldBuilder.xml"),
                ["C:\\test\\Summary.xml"] = LoadDocumentFromResource("csmacnz.Coveralls.Tests.ReportGeneratorSample.Summary.xml"),
            };

            var results = CreateReportGeneratorParser().GenerateSourceFiles(documents, false);

            Assert.NotNull(results);
        }
        
        private ReportGeneratorParser CreateReportGeneratorParser()
        {
            return new ReportGeneratorParser(new PathProcessor(""));
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