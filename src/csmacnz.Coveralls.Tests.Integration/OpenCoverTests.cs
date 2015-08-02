using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Xunit;

namespace csmacnz.Coveralls.Tests.Integration
{
    public class OpenCoverTests
    {
        [Fact]
        public void EmptyReport_RunsSuccessfully()
        {
            var emptyFilePath = Path.Combine(RepositoryPaths.GetSamplesPath(), "opencover", "Sample1", "EmptyReport.xml");
            var results = DryRunCoverallsWithInputFile(emptyFilePath);

            Assert.Equal(0, results.ExitCode);
        }

        [Fact]
        public void ReportWithOneFile_RunsSuccessfully()
        {
            string sampleFolderPath = Path.Combine(RepositoryPaths.GetSamplesPath(), "opencover", "Sample2");
            var sampleCoverageFile = Path.Combine(sampleFolderPath, "SingleFileReport.xml");
            var sampleClassFile = Path.Combine(sampleFolderPath, "SingleFileReportSourceFile.txt");
            var coverageFilePath = TestFolders.GetTempFilePath(Guid.NewGuid() + ".xml");
            var classFilePath = TestFolders.GetTempFilePath(Guid.NewGuid() + ".cs");
            File.Copy(sampleClassFile, classFilePath);
            var doc = XDocument.Load(sampleCoverageFile);
            var classFile = doc.XPathSelectElements("//CoverageSession/Modules/Module/Files/File").FirstOrDefault(e => e.Attribute("fullPath").Value.EndsWith("Class1.cs"));
            classFile.Attribute("fullPath").SetValue(classFilePath);
            doc.Save(coverageFilePath);

            var results = DryRunCoverallsWithInputFile(coverageFilePath);

            Assert.Equal(0, results.ExitCode);
        }

        private static CoverageRunResults DryRunCoverallsWithInputFile(string inputFilePath)
        {
            return CoverallsTestRunner.RunCoveralls(string.Format("--opencover -i {0} --dryrun --repoToken MYTESTREPOTOKEN", inputFilePath));
        }
    }
}
