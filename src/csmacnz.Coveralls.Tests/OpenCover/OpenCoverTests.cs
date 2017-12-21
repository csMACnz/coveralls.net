using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using csmacnz.Coveralls.Tests.TestAdapters;
using csmacnz.Coveralls.Tests.TestHelpers;
using Xunit;

namespace csmacnz.Coveralls.Tests.OpenCover
{
    public class OpenCoverTests
    {
        [Fact]
        public void EmptyReport_RunsSuccessfully()
        {
            var fileSystem = new TestFileSystem();
            var filePath = Path.Combine(RepositoryPaths.GetSamplesPath(), "opencover", "Sample1", "EmptyReport.xml");
            fileSystem.AddFile(filePath, Reports.OpenCoverSamples.EmptyReport);

            var results = DryRunCoverallsWithInputFile(filePath, fileSystem);

            Assert.Equal(0, results.ExitCode);
        }

        [Fact]
        public void EmptyReport_MultipleMode_RunsSuccessfully()
        {
            var fileSystem = new TestFileSystem();
            var filePath = Path.Combine(RepositoryPaths.GetSamplesPath(), "opencover", "Sample1", "EmptyReport.xml");
            fileSystem.AddFile(filePath, Reports.OpenCoverSamples.EmptyReport);

            var results = DryRunCoverallsMultiModeWithInputFile(filePath, fileSystem);

            Assert.Equal(0, results.ExitCode);
        }

        [Fact]
        public void ReportWithOneFile_RunsSuccessfully()
        {
            var (fileSystem, coverageFilePath) = BuildReportWithOneFile();

            var results = DryRunCoverallsWithInputFile(coverageFilePath, fileSystem);

            Assert.Equal(0, results.ExitCode);
        }

        [Fact]
        public void ReportWithOneFile_MultipleMode_RunsSuccessfully()
        {
            var (fileSystem, coverageFilePath) = BuildReportWithOneFile();

            var results = DryRunCoverallsMultiModeWithInputFile(coverageFilePath, fileSystem);

            Assert.Equal(0, results.ExitCode);
        }

        private static (TestFileSystem, string basePath) BuildReportWithOneFile()
        {
            var fileSystem = new TestFileSystem();
            string filePath = TestFileSystem.GenerateRandomAbsolutePath("opencover");

            string sourcePath = Path.Combine(filePath, "SingleFileReportSourceFile.txt");
            fileSystem.AddFile(sourcePath, Reports.OpenCoverSamples.SingleFileReportSourceFile);

            var doc = XDocument.Parse(Reports.OpenCoverSamples.SingleFileReportOneLineCovered);
            var classFile = doc
                .XPathSelectElements("//CoverageSession/Modules/Module/Files/File")
                .FirstOrDefault(e => e.Attribute("fullPath").Value.EndsWith("Class1.cs", StringComparison.Ordinal));
            classFile.Attribute("fullPath").SetValue(sourcePath);

            var reportContents = doc.ToString();
            string reportPath = Path.Combine(filePath, "SingleFileReportOneLineCovered.xml");
            fileSystem.AddFile(reportPath, reportContents);

            return (fileSystem, reportPath);
        }

        private static CoverallsRunResults DryRunCoverallsWithInputFile(
            string inputFilePath,
            TestFileSystem testFileSystem)
        {
            return CoverallsTestRunner.RunCoveralls(
                $"--opencover -i {inputFilePath} --dryrun --repoToken MYTESTREPOTOKEN",
                testFileSystem);
        }

        private static CoverallsRunResults DryRunCoverallsMultiModeWithInputFile(
            string inputFilePath,
            TestFileSystem testFileSystem)
        {
            return CoverallsTestRunner.RunCoveralls(
                $"--multiple -i opencover={inputFilePath} --dryrun --repoToken MYTESTREPOTOKEN",
                testFileSystem);
        }
    }
}
