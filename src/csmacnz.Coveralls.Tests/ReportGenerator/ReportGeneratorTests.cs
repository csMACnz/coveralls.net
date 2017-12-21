using System;
using System.IO;
using csmacnz.Coveralls.Tests.TestAdapters;
using csmacnz.Coveralls.Tests.TestHelpers;
using Xunit;

namespace csmacnz.Coveralls.Tests.OpenCover
{
    public class ReportGeneratorTests
    {
        [Fact]
        public void MissingFolder_RunsWithCorrectErrorMessage()
        {
            var fileSystem = new TestFileSystem();
            var directoryPath = TestFileSystem.GenerateRandomAbsolutePath("reports", "Sample1");

            var results = DryRunCoverallsWithInputFile(directoryPath, fileSystem);

            Assert.Equal(1, results.ExitCode);
            Assert.Equal(
                $"Input file '{directoryPath}' cannot be found",
                results.StandardError);
        }

        [Fact]
        public void EmptyReport_RunsSuccessfully()
        {
            var fileSystem = new TestFileSystem();
            var directoryPath = TestFileSystem.GenerateRandomAbsolutePath("reports", "Sample1");
            fileSystem.AddFile(Path.Combine(directoryPath, "Summary.xml"), Reports.ReportGeneratorSample.Sample1.Summary);
            fileSystem.AddFile(Path.Combine(directoryPath, "test_test.UnitTest1.xml"), Reports.ReportGeneratorSample.Sample1.Test_test_UnitTest1);

            var results = DryRunCoverallsWithInputFile(directoryPath, fileSystem);

            Assert.Equal(0, results.ExitCode);
        }

        [Fact]
        public void EmptyReport_MultipleMode_RunsSuccessfully()
        {
            var fileSystem = new TestFileSystem();
            var directoryPath = TestFileSystem.GenerateRandomAbsolutePath("reports", "Sample1");
            fileSystem.AddFile(Path.Combine(directoryPath, "Summary.xml"), Reports.ReportGeneratorSample.Sample1.Summary);
            fileSystem.AddFile(Path.Combine(directoryPath, "test_test.UnitTest1.xml"), Reports.ReportGeneratorSample.Sample1.Test_test_UnitTest1);

            var results = DryRunCoverallsMultiModeWithInputFile(directoryPath, fileSystem);

            Assert.Equal(0, results.ExitCode);
        }

        private static CoverallsRunResults DryRunCoverallsWithInputFile(
            string directoryPath,
            TestFileSystem testFileSystem)
        {
            return CoverallsTestRunner.RunCoveralls(
                $"--reportgenerator -i {directoryPath} --dryrun --repoToken MYTESTREPOTOKEN",
                testFileSystem);
        }

        private static CoverallsRunResults DryRunCoverallsMultiModeWithInputFile(
            string directoryPath,
            TestFileSystem testFileSystem)
        {
            return CoverallsTestRunner.RunCoveralls(
                $"--multiple -i reportgenerator={directoryPath} --dryrun --repoToken MYTESTREPOTOKEN",
                testFileSystem);
        }
    }
}
