using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using csmacnz.Coveralls.Tests.TestAdapters;
using csmacnz.Coveralls.Tests.TestHelpers;
using Xunit;

namespace csmacnz.Coveralls.Tests.NCover
{
    public class NCoverTests
    {
        [Fact]
        public void EmptyReport_RunsSuccessfully()
        {
            var fileSystem = new TestFileSystem();
            string filePath = TestFileSystem.GenerateRandomAbsolutePath("ncover", "EmptyReport.xml");
            fileSystem.AddFile(filePath, Reports.NCoverSamples.EmptyReport);

            var results = DryRunCoverallsWithInputFile(filePath, fileSystem);

            CoverallsAssert.RanSuccessfully(results);
        }

        [Fact]
        public void EmptyReport_MultipleMode_RunsSuccessfully()
        {
            var fileSystem = new TestFileSystem();
            string filePath = TestFileSystem.GenerateRandomAbsolutePath("ncover", "EmptyReport.xml");
            fileSystem.AddFile(filePath, Reports.NCoverSamples.EmptyReport);

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

        private static (TestFileSystem FileSystem, string BasePath) BuildReportWithOneFile()
        {
            var fileSystem = new TestFileSystem();
            string filePath = TestFileSystem.GenerateRandomAbsolutePath("ncover");

            string sourcePath = Path.Combine(filePath, "SingleFileReportSourceFile.txt");
            fileSystem.AddFile(sourcePath, Reports.NCoverSamples.SingleFileReportOneLineCovered.SourceFile);

            var doc = XDocument.Parse(Reports.NCoverSamples.SingleFileReportOneLineCovered.Report);
            var classFiles =
                doc.XPathSelectElements("//coverage/module/method/seqpnt")
                    .Where(e => e.Attribute("document")!.Value.EndsWith("Class1.cs", StringComparison.Ordinal));
            foreach (var classFile in classFiles)
            {
                classFile.Attribute("document")!.SetValue(sourcePath);
            }

            var reportContents = doc.ToString();
            string reportPath = Path.Combine(filePath, "SingleFileReportOneLineCovered.xml");
            fileSystem.AddFile(reportPath, reportContents);

            return (fileSystem, reportPath);
        }

        private static CoverallsRunResults DryRunCoverallsWithInputFile(string inputFilePath, TestFileSystem testFileSystem)
        {
            return CoverallsTestRunner.RunCoveralls(
                $"--ncover -i {inputFilePath} --dryrun --repoToken MYTESTREPOTOKEN",
                testFileSystem);
        }

        private static CoverallsRunResults DryRunCoverallsMultiModeWithInputFile(string inputFilePath, TestFileSystem testFileSystem)
        {
            return CoverallsTestRunner.RunCoveralls(
                $"--multiple -i ncover={inputFilePath} --dryrun --repoToken MYTESTREPOTOKEN",
                testFileSystem);
        }
    }
}
