using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Xunit;

namespace csmacnz.Coveralls.Tests
{
    public class NCoverTests
    {
        [Fact]
        public void EmptyReport_RunsSuccessfully()
        {
            var emptyFilePath = Path.Combine(RepositoryPaths.GetSamplesPath(), "ncover", "Sample1", "EmptyReport.xml");
            var results = DryRunCoverallsWithInputFile(emptyFilePath);

            Assert.Equal(0, results.ExitCode);
        }

        [Fact]
        public void EmptyReport_MultipleMode_RunsSuccessfully()
        {
            var emptyFilePath = Path.Combine(RepositoryPaths.GetSamplesPath(), "ncover", "Sample1", "EmptyReport.xml");
            var results = DryRunCoverallsMultiModeWithInputFile(emptyFilePath);

            Assert.Equal(0, results.ExitCode);
        }

        [Fact]
        public void ReportWithOneFile_RunsSuccessfully()
        {
            var coverageFilePath = BuildReportWithOneFile();

            var results = DryRunCoverallsWithInputFile(coverageFilePath);

            Assert.Equal(0, results.ExitCode);
        }

        [Fact]
        public void ReportWithOneFile_MultipleMode_RunsSuccessfully()
        {
            var coverageFilePath = BuildReportWithOneFile();

            var results = DryRunCoverallsMultiModeWithInputFile(coverageFilePath);

            Assert.Equal(0, results.ExitCode);
        }

        private static string BuildReportWithOneFile()
        {
            var sampleFolderPath = Path.Combine(RepositoryPaths.GetSamplesPath(), "ncover", "Sample2");
            var sampleCoverageFile = Path.Combine(sampleFolderPath, "SingleFileReportOneLineCovered.xml");
            var sampleClassFile = Path.Combine(sampleFolderPath, "SingleFileReportSourceFile.txt");
            var coverageFilePath = TestFolders.GetTempFilePath(Guid.NewGuid() + ".xml");
            var classFilePath = TestFolders.GetTempFilePath(Guid.NewGuid() + ".cs");
            File.Copy(sampleClassFile, classFilePath);
            var doc = XDocument.Load(sampleCoverageFile);
            var classFiles =
                doc.XPathSelectElements("//coverage/module/method/seqpnt")
                    .Where(e => e.Attribute("document").Value.EndsWith("Class1.cs", StringComparison.Ordinal));
            foreach (var classFile in classFiles)
            {
                classFile.Attribute("document").SetValue(classFilePath);
            }

            using (var stream = File.OpenWrite(coverageFilePath))
            {
                doc.Save(stream);
            }

            return coverageFilePath;
        }

        private static CoverallsRunResults DryRunCoverallsWithInputFile(string inputFilePath)
        {
            return CoverallsTestRunner.RunCoveralls(
                $"--ncover -i {inputFilePath} --dryrun --repoToken MYTESTREPOTOKEN");
        }

        private static CoverallsRunResults DryRunCoverallsMultiModeWithInputFile(string inputFilePath)
        {
            return CoverallsTestRunner.RunCoveralls(
                $"--multiple -i ncover={inputFilePath} --dryrun --repoToken MYTESTREPOTOKEN");
        }
    }
}
