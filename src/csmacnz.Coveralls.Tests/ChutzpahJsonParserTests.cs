using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using csmacnz.Coveralls.Parsers;
using NSubstitute;
using Xunit;

namespace csmacnz.Coveralls.Tests
{
    public class ChutzpahJsonParserTests
    {
        private IFileSystem _fileSystem;
        private IParser _parser;

        [Fact]
        public void GenerateSourceFiles_NonRelativePath()
        {
            Initizliaze();
            _fileSystem.GetCorrectCaseOfParentFolder(Arg.Any<string>()).Returns(x => x[0]);
            _fileSystem.ReadAllText("file").Returns(CHUTZPAH_JSON_EXMAPLE);

            var results = _parser.GenerateSourceFiles("file", false);

            Assert.Equal(2, results.Count);
            Assert.Equal("D:\\path\\to\\file\\file.ts", results.First().Name);
            Assert.Equal(36, results.First().Coverage[0]);
            Assert.Equal(10, results.Last().Coverage[5]);
            Assert.Equal(null, results.First().Coverage[7]);
        }

        [Fact]
        public void GenerateSourceFiles_RelativePath()
        {
            Initizliaze();
            _fileSystem.GetCorrectCaseOfParentFolder(Arg.Any<string>()).Returns(x => x[0]);
            _fileSystem.ReadAllText("file").Returns(CHUTZPAH_JSON_EXMAPLE);

            var results = _parser.GenerateSourceFiles("file", true);

            Assert.Equal(2, results.Count);
            Assert.Equal("file.ts", results.First().Name);
            Assert.Equal(36, results.First().Coverage[0]);
            Assert.Equal(10, results.First().Coverage[5]);
            Assert.Equal(null, results.First().Coverage[7]);
        }


        private const string CHUTZPAH_JSON_EXMAPLE =
            @"{
                ""D:\\path\\to\\file\\file.ts"" :{
                    ""FilePath"":""D:\\path\\to\\file\\file.ts"",
                    ""LineExecutionCounts"":[
                         null,
                         36,
                         18,
                         null,
                         null,
                         9,
                         10,
                         null,
                         null,
                         null,
                         null,
                         null,
                         null,
                         18,
                         18,
                         null
                      ],
                    ""SourceLines"":[
                         ""namespace IsraelHiking.Services {"",
                         ""    export class MapService {"",
                         ""        public map: L.Map;"",
                         """",
                         ""        constructor() {"",
                         ""            this.map = L.map(\""map\"", {"",
                         ""                center: L.latLng(31.773, 35.12),"",
                         ""                zoom: 13,"",
                         ""                doubleClickZoom: false,"",
                         ""                zoomControl: false"",
                         ""            } as L.Map.MapOptions);"",
                         ""        }"",
                         ""    }"",
                         ""}"",
                         "" ""
                      ],
                    ""CoveragePercentage"":1
                    },
""D:\\path\\to\\file\\file2.ts"" :{
                    ""FilePath"":""D:\\path\\to\\file\\file2.ts"",
                    ""LineExecutionCounts"":[
                         null,
                         36,
                         18,
                         null,
                         null,
                         9,
                         10,
                         null,
                         null,
                         null,
                         null,
                         null,
                         null,
                         18,
                         18,
                         null
                      ],
                    ""SourceLines"":[
                         ""namespace IsraelHiking.Services {"",
                         ""    export class MapService {"",
                         ""        public map: L.Map;"",
                         """",
                         ""        constructor() {"",
                         ""            this.map = L.map(\""map\"", {"",
                         ""                center: L.latLng(31.773, 35.12),"",
                         ""                zoom: 13,"",
                         ""                doubleClickZoom: false,"",
                         ""                zoomControl: false"",
                         ""            } as L.Map.MapOptions);"",
                         ""        }"",
                         ""    }"",
                         ""}"",
                         "" ""
                      ],
                    ""CoveragePercentage"":1
                    }
            }";


        private void Initizliaze()
        {
            _fileSystem = Substitute.For<IFileSystem>();
            _parser = new ChutzpahJsonParser(new PathProcessor(@"D:\path\to\file\"), _fileSystem); 
        }
    }
}
