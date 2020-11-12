﻿using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace csmacnz.Coveralls.Tests.Integration
{
    /*
     * http://gasparnagy.com/2014/01/integration-testing-tips-input-and-output-files/
     */

    internal static class TestFolders
    {
        public static readonly string UniqueId = DateTime.Now.ToString("s", CultureInfo.InvariantCulture)
            .Replace(":", string.Empty, StringComparison.InvariantCulture);

        public static string InputFolder
            => Path.GetDirectoryName(new Uri(typeof(CoverallsTestRunner).GetTypeInfo().Assembly.Location!).LocalPath) !;

        public static string OutputFolder
        {
            get
            {
                var outputFolder = Path.Combine(Directory.GetCurrentDirectory(), UniqueId);
                if (!Directory.Exists(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                }

                return outputFolder;
            }
        }

        public static string TempFolder => Path.GetTempPath();

        // very simple helper methods that can improve the test code readability
        public static string GetInputFilePath(string fileName)
        {
            return Path.GetFullPath(Path.Combine(InputFolder, fileName));
        }

        public static string GetOutputFilePath(string fileName)
        {
            return Path.GetFullPath(Path.Combine(OutputFolder, fileName));
        }

        public static string GetTempFilePath(string fileName)
        {
            return Path.GetFullPath(Path.Combine(TempFolder, fileName));
        }
    }
}
