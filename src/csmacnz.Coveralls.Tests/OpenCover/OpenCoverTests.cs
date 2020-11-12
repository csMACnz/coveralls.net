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
        public void EmptyReport_GivenAnOutput_OutputsSamplePost()
        {
            var fileSystem = new TestFileSystem();
            var outfile = "TestingOutput.xml";
            var filePath = TestFileSystem.GenerateRandomAbsolutePath("opencover", "Sample1", "EmptyReport.xml");
            fileSystem.AddFile(filePath, Reports.OpenCoverSamples.EmptyReport);

            var results = CoverallsTestRunner.RunCoveralls(
                $"--opencover -i {filePath} --dryrun --output {outfile} --repoToken MYTESTREPOTOKEN",
                fileSystem);

            CoverallsAssert.RanSuccessfully(results);
            var savedFile = fileSystem.TryLoadFile(outfile);
            Assert.True(savedFile.HasValue, "Expected file to exist in fileSystem");
            var savedFileData = savedFile.ValueOr(" ");
            Assert.Contains(@"""repo_token"":""MYTESTREPOTOKEN""", savedFileData, StringComparison.Ordinal);
            Assert.Contains(@"""service_name"":""coveralls.net""", savedFileData, StringComparison.Ordinal);
            Assert.Contains(@"""parallel"":false", savedFileData, StringComparison.Ordinal);
            Assert.Contains(@"""source_files"":[]", savedFileData, StringComparison.Ordinal);
        }

        [Fact]
        public void EmptyReport_RunsSuccessfully()
        {
            var fileSystem = new TestFileSystem();
            var filePath = TestFileSystem.GenerateRandomAbsolutePath("opencover", "Sample1", "EmptyReport.xml");
            fileSystem.AddFile(filePath, Reports.OpenCoverSamples.EmptyReport);

            var results = DryRunCoverallsWithInputFile(filePath, fileSystem);

            CoverallsAssert.RanSuccessfully(results);
        }

        [Fact]
        public void EmptyReport_MultipleMode_RunsSuccessfully()
        {
            var fileSystem = new TestFileSystem();
            var filePath = TestFileSystem.GenerateRandomAbsolutePath("opencover", "Sample1", "EmptyReport.xml");
            fileSystem.AddFile(filePath, Reports.OpenCoverSamples.EmptyReport);

            var results = DryRunCoverallsMultiModeWithInputFile(filePath, fileSystem);

            CoverallsAssert.RanSuccessfully(results);
        }

        [Fact]
        public void ReportWithOneFile_RunsSuccessfully()
        {
            var (fileSystem, coverageFilePath) = BuildReportWithOneFile();

            var results = DryRunCoverallsWithInputFile(coverageFilePath, fileSystem);

            CoverallsAssert.RanSuccessfully(results);
        }

        [Fact]
        public void ReportWithOneFile_MultipleMode_RunsSuccessfully()
        {
            var (fileSystem, coverageFilePath) = BuildReportWithOneFile();

            var results = DryRunCoverallsMultiModeWithInputFile(coverageFilePath, fileSystem);

            CoverallsAssert.RanSuccessfully(results);
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
                .FirstOrDefault(e => e.Attribute(XName.Get("fullPath")) !.Value.EndsWith("Class1.cs", StringComparison.Ordinal));
            classFile!.Attribute(XName.Get("fullPath")) !.SetValue(sourcePath);

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
