using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using csmacnz.Coveralls.Parsers;
using Xunit;
using System.Collections.Generic;

namespace csmacnz.Coveralls.Tests
{
    public class ChutzpahJsonParserTests
    {
        [Fact]
        public void GenerateSourceFiles_CorrectCoverage()
        {
            var fileContents = LoadFromResource("csmacnz.Coveralls.Tests.ChutzpahExample.json");

            var results = ChutzpahJsonParser.GenerateSourceFiles(fileContents);

            Assert.Equal(2, results.Count);
            Assert.Equal(@"D:\path\to\file\file.ts", results.First().FullPath);
            Assert.Equal(36, results.First().Coverage[0]);
            Assert.Equal(10, results.First().Coverage[5]);
            Assert.Equal(null, results.First().Coverage[7]);
        }
        
        private static string[] LoadFromResource(string embeddedResource)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            List<string> contents = new List<string>();
            using (var stream = executingAssembly.GetManifestResourceStream(embeddedResource))
            {
                Assert.NotNull(stream);
                Debug.Assert(stream != null, "stream != null");
                using (var reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        contents.Add(line);
                    }
                }
            }
            return contents.ToArray();
        }
    }
}