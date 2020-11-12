﻿using System;
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
            var emptyFilePath = Path.Combine(RepositoryPaths.GetSamplesPath(), "OpenCover", "EmptyReport.xml");
            var results = DryRunCoverallsWithInputFile(emptyFilePath);

            CoverallsAssert.RanSuccessfully(results);
        }

        [Fact]
        public void EmptyReport_MultipleMode_RunsSuccessfully()
        {
            var emptyFilePath = Path.Combine(RepositoryPaths.GetSamplesPath(), "OpenCover", "EmptyReport.xml");
            var results = DryRunCoverallsMultiModeWithInputFile(emptyFilePath);

            CoverallsAssert.RanSuccessfully(results);
        }

        [Fact]
        public void ReportWithOneFile_RunsSuccessfully()
        {
            var coverageFilePath = BuildReportWithOneFile();

            var results = DryRunCoverallsWithInputFile(coverageFilePath);

            CoverallsAssert.RanSuccessfully(results);
        }

        [Fact]
        public void ReportWithOneFile_MultipleMode_RunsSuccessfully()
        {
            var coverageFilePath = BuildReportWithOneFile();

            var results = DryRunCoverallsMultiModeWithInputFile(coverageFilePath);

            CoverallsAssert.RanSuccessfully(results);
        }

        private static string BuildReportWithOneFile()
        {
            var sampleFolderPath = Path.Combine(RepositoryPaths.GetSamplesPath(), "OpenCover");
            var sampleCoverageFile = Path.Combine(sampleFolderPath, "SingleFileReport.xml");
            var sampleClassFile = Path.Combine(sampleFolderPath, "SingleFileReportSourceFile.txt");
            var coverageFilePath = TestFolders.GetTempFilePath(Guid.NewGuid() + ".xml");
            var classFilePath = TestFolders.GetTempFilePath(Guid.NewGuid() + ".cs");
            File.Copy(sampleClassFile, classFilePath);
            var doc = XDocument.Load(sampleCoverageFile);
            var classFile =
                doc.XPathSelectElements("//CoverageSession/Modules/Module/Files/File")
                    .FirstOrDefault(e => e.Attribute(XName.Get("fullPath")) !.Value.EndsWith("Class1.cs", StringComparison.Ordinal));
            classFile!.Attribute(XName.Get("fullPath")) !.SetValue(classFilePath);
            using (var stream = File.OpenWrite(coverageFilePath))
            {
                doc.Save(stream);
            }

            return coverageFilePath;
        }

        private static CoverallsRunResults DryRunCoverallsWithInputFile(string inputFilePath)
        {
            return CoverallsTestRunner.RunCoveralls(
                $"--opencover -i {inputFilePath} --dryrun --repoToken MYTESTREPOTOKEN");
        }

        private static CoverallsRunResults DryRunCoverallsMultiModeWithInputFile(string inputFilePath)
        {
            return CoverallsTestRunner.RunCoveralls(
                $"--multiple -i opencover={inputFilePath} --dryrun --repoToken MYTESTREPOTOKEN");
        }
    }
}
