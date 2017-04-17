using System.Collections.Generic;
using System.Text.RegularExpressions;
using csmacnz.Coveralls.Data;

namespace csmacnz.Coveralls.Parsers
{
    public static class LcovParser
    {
        public static List<FileCoverageData> GenerateSourceFiles(string[] lines)
        {
            FileCoverageDataBuilder coverageBuilder = null;
            var files = new List<FileCoverageData>();
            foreach (var line in lines)
            {
                var matches = Regex.Match(line, "^SF:(.*)");
                if (matches.Success)
                {
                    coverageBuilder = new FileCoverageDataBuilder(matches.Groups[1].Value);
                    continue;
                }
                matches = Regex.Match(line, @"^DA:(\d+),(\d+)");
                if (matches.Success)
                {
                    if (coverageBuilder != null)
                    {
                        var lineNumber = int.Parse(matches.Groups[1].Value);
                        var coverageNumber = int.Parse(matches.Groups[2].Value);
                        coverageBuilder.RecordCoverage(lineNumber, coverageNumber);
                    }
                    continue;
                }
                if (line.Equals("end_of_record"))
                {
                    if (coverageBuilder != null)
                    {
                        files.Add(coverageBuilder.CreateFile());
                        coverageBuilder = null;
                    }
                }
            }
            return files;
        }
    }
}