using System.Reflection;
using System.Runtime.CompilerServices;
using csMACnz.EmbedRes;

namespace csmacnz.Coveralls.Tests
{
    public static class Reports
    {
        public static string GetFileContents(string resourceName)
        {
            return ResourceLoader.GetContentFromFolderMatchingTypeName(
                targetTypeInfo: typeof(Reports).GetTypeInfo(),
                resourceFilename: resourceName);
        }

        public static class ChutzpahSample
        {
            public static string ChutzpahExample => Reports.GetFileContents("Chutzpah.ChutzpahExample.json");
        }

        public static class ReportGeneratorSample
        {
            public static class Sample1
            {
                public static string Summary => GetReportGeneratorContents("Summary");

                public static string Test_test_UnitTest1 => GetReportGeneratorContents("test_test.UnitTest1");

                public static string GetReportGeneratorContents(string resourceName)
                {
                    return Reports.GetFileContents($"ReportGenerator.Sample1.{resourceName}.xml");
                }
            }

            public static class Sample2
            {
                public static string GameOfLife_xUnit_Tests_GameOfLife_xUnit_Tests_WorldTests => GetReportGeneratorContents("GameOfLife.xUnit.Tests_GameOfLife.xUnit.Tests.WorldTests");

                public static string GameOfLife_GameOfLife_Game => GetReportGeneratorContents("GameOfLife_GameOfLife.Game");

                public static string GameOfLife_GameOfLife_Program => GetReportGeneratorContents("GameOfLife_GameOfLife.Program");

                public static string GameOfLife_GameOfLife_World => GetReportGeneratorContents("GameOfLife_GameOfLife.World");

                public static string GameOfLife_GameOfLife_WorldBuilder => GetReportGeneratorContents("GameOfLife_GameOfLife.WorldBuilder");

                public static string Summary => GetReportGeneratorContents("Summary");

                public static string GetReportGeneratorContents(string resourceName)
                {
                    return Reports.GetFileContents($"ReportGenerator.Sample2.{resourceName}.xml");
                }
            }
        }

        public static class NCoverSamples
        {
            public static string EmptyReport => GetNCoverContents("Sample1.EmptyReport.xml");

            public static class SingleFileReportOneLineCovered
            {
                public static string Report => GetNCoverContents("Sample2.SingleFileReportOneLineCovered.xml");

                public static string SourceFile => GetNCoverContents("Sample2.SingleFileReportSourceFile.txt");
            }

            public static string SingleFileReportOneLineUncovered => GetNCoverContents("Sample3.SingleFileReportOneLineUncovered.Xml");

            public static string GetNCoverContents(string resourceName)
            {
                return Reports.GetFileContents($"NCover.{resourceName}");
            }
        }

        public static class OpenCoverSamples
        {
            public static string EmptyReport => GetOpenCoverXmlContents();

            public static string SingleFileReport => GetOpenCoverXmlContents();

            public static string SingleFileReportOneLineCovered => GetOpenCoverXmlContents();

            public static string SingleFileReportOneLineUncovered => GetOpenCoverXmlContents();

            public static string SingleFileReportSourceFile => GetOpenCoverContents("SingleFileReportSourceFile.txt");

            public static string GetOpenCoverXmlContents([CallerMemberName] string resourceName = null)
            {
                return GetOpenCoverContents($"{resourceName}.xml");
            }

            public static string GetOpenCoverContents(string resourceName)
            {
                return Reports.GetFileContents($"OpenCover.{resourceName}");
            }
        }
    }
}
