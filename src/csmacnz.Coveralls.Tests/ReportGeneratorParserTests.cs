using System.Diagnostics;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using csmacnz.Coveralls.Parsers;
using Xunit;
using System.Collections.Generic;
using System.Linq;

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

            var results = CreateReportGeneratorParser().GenerateSourceFiles(documents);

            Assert.NotNull(results);
            Assert.Equal(5, results.Count);
            Assert.Equal(@"C:\dev\Coveralls.net-Samples\src\GameOfLife.xUnit.Tests\WorldTests.cs", results[0].FullPath);
            Assert.Equal(10, results[0].Coverage.Sum());
            Assert.Equal(@"C:\dev\Coveralls.net-Samples\src\GameOfLife\Game.cs", results[1].FullPath);
            Assert.Equal(0, results[1].Coverage.Sum());
            Assert.Equal(@"C:\dev\Coveralls.net-Samples\src\GameOfLife\Program.cs", results[2].FullPath);
            Assert.Equal(0, results[2].Coverage.Sum());
            Assert.Equal(@"C:\dev\Coveralls.net-Samples\src\GameOfLife\World.cs", results[3].FullPath);
            Assert.Equal(18, results[3].Coverage.Sum());
            Assert.Equal(@"C:\dev\Coveralls.net-Samples\src\GameOfLife\WorldBuilder.cs", results[4].FullPath);
            Assert.Equal(0, results[4].Coverage.Sum());
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