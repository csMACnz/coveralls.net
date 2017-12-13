using System.Reflection;
using System.Runtime.CompilerServices;
using csMACnz.EmbedRes;

namespace csmacnz.Coveralls.Tests
{
    public static class Reports
    {
        public static string ChutzpahExample => GetJsonContents();

        public static string EmptyReport => GetXmlContents();

        public static string EmptyReportNcover => GetXmlContents();

        public static string SingleFileReport => GetXmlContents();

        public static string SingleFileReportOneLineCovered => GetXmlContents();

        public static string SingleFileReportOneLineUncovered => GetXmlContents();

        public static string SingleFileReportOneLineCoveredNcover => GetXmlContents();

        public static string SingleFileReportOneLineUncoveredNcover => GetXmlContents();

        public static string SingleFileReportSourceFile => GetFileContents("SingleFileReportSourceFile.txt");

        public static string GetJsonContents([CallerMemberName] string resourceName = null)
        {
            return GetFileContents($"{resourceName}.json");
        }

        public static string GetXmlContents([CallerMemberName] string resourceName = null)
        {
            return GetFileContents($"{resourceName}.xml");
        }

        public static string GetFileContents(string resourceName)
        {
            return ResourceLoader.GetContentFromFolderMatchingTypeName(
                targetTypeInfo: typeof(Reports).GetTypeInfo(),
                resourceFilename: resourceName);
        }

        public static class ReportGeneratorSample
        {
            public static string GameOfLife_xUnit_Tests_GameOfLife_xUnit_Tests_WorldTests => GetReportGeneratorContents("GameOfLife.xUnit.Tests_GameOfLife.xUnit.Tests.WorldTests");

            public static string GameOfLife_GameOfLife_Game => GetReportGeneratorContents("GameOfLife_GameOfLife.Game");

            public static string GameOfLife_GameOfLife_Program => GetReportGeneratorContents("GameOfLife_GameOfLife.Program");

            public static string GameOfLife_GameOfLife_World => GetReportGeneratorContents("GameOfLife_GameOfLife.World");

            public static string GameOfLife_GameOfLife_WorldBuilder => GetReportGeneratorContents("GameOfLife_GameOfLife.WorldBuilder");

            public static string Summary => GetReportGeneratorContents("Summary");

            public static string GetReportGeneratorContents(string resourceName)
            {
                return Reports.GetFileContents($"ReportGeneratorSample.{resourceName}.xml");
            }
        }
    }
}
