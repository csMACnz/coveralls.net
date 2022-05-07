using System.IO;
using csmacnz.Coveralls.Tests.TestAdapters;
using csmacnz.Coveralls.Tests.TestHelpers;
using Xunit;

namespace csmacnz.Coveralls.Tests.OpenCover;

public class ReportGeneratorTests
{
    [Fact]
    public void MissingFolder_RunsWithCorrectErrorMessage()
    {
        var directoryPath = TestFileSystem.GenerateRandomAbsolutePath("reports", "Sample1");
        var fileSystem = new TestFileSystem();

        var results = DryRunCoverallsWithInputFile(directoryPath, fileSystem);

        Assert.Equal(1, results.ExitCode);
        Assert.Equal(
            $"Input file '{directoryPath}' cannot be found",
            results.StandardError);
    }

    [Fact]
    public void EmptyReport_RunsSuccessfully()
    {
        var directoryPath = TestFileSystem.GenerateRandomAbsolutePath("reports", "Sample1");
        var fileSystem = EmptyReportFileSystem(directoryPath);

        var results = DryRunCoverallsWithInputFile(directoryPath, fileSystem);

        CoverallsAssert.RanSuccessfully(results);
    }

    [Fact]
    public void EmptyReport_MultipleMode_RunsSuccessfully()
    {
        var directoryPath = TestFileSystem.GenerateRandomAbsolutePath("reports", "Sample1");
        var fileSystem = EmptyReportFileSystem(directoryPath);

        var results = DryRunCoverallsMultiModeWithInputFile(directoryPath, fileSystem);

        CoverallsAssert.RanSuccessfully(results);
    }

    private static TestFileSystem EmptyReportFileSystem(string directoryPath)
    {
        var fileSystem = new TestFileSystem();
        fileSystem.AddFile(Path.Combine(directoryPath, "Summary.xml"), Reports.ReportGeneratorSample.Sample1.Summary);
        fileSystem.AddFile(Path.Combine(directoryPath, "test_test.UnitTest1.xml"), Reports.ReportGeneratorSample.Sample1.Test_test_UnitTest1);

        return fileSystem;
    }

    private static CoverallsRunResults DryRunCoverallsWithInputFile(
        string directoryPath,
        TestFileSystem testFileSystem) => CoverallsTestRunner.RunCoveralls(
            $"--reportgenerator -i {directoryPath} --dryrun --repoToken MYTESTREPOTOKEN",
            testFileSystem);

    private static CoverallsRunResults DryRunCoverallsMultiModeWithInputFile(
        string directoryPath,
        TestFileSystem testFileSystem) => CoverallsTestRunner.RunCoveralls(
            $"--multiple -i reportgenerator={directoryPath} --dryrun --repoToken MYTESTREPOTOKEN",
            testFileSystem);
}
