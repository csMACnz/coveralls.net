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
                ["C:\\test\\GameOfLife.xUnit.Tests_GameOfLife.xUnit.Tests.WorldTests.xml"] = XDocument.Parse(Reports.ReportGeneratorSample.GameOfLife_xUnit_Tests_GameOfLife_xUnit_Tests_WorldTests),
                ["C:\\test\\GameOfLife_GameOfLife.Game.xml"] = XDocument.Parse(Reports.ReportGeneratorSample.GameOfLife_GameOfLife_Game),
                ["C:\\test\\GameOfLife_GameOfLife.Program.xml"] = XDocument.Parse(Reports.ReportGeneratorSample.GameOfLife_GameOfLife_Program),
                ["C:\\test\\GameOfLife_GameOfLife.World.xml"] = XDocument.Parse(Reports.ReportGeneratorSample.GameOfLife_GameOfLife_World),
                ["C:\\test\\GameOfLife_GameOfLife.WorldBuilder.xml"] = XDocument.Parse(Reports.ReportGeneratorSample.GameOfLife_GameOfLife_WorldBuilder),
                ["C:\\test\\Summary.xml"] = XDocument.Parse(Reports.ReportGeneratorSample.Summary),
            };

            var results = ReportGeneratorParser.GenerateSourceFiles(documents);

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
    }
}