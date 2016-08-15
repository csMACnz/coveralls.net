using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace csmacnz.Coveralls.Tests
{
    public class ChutzpahJsonParserTests
    {
        [Fact]
        public void EmptyReportLoadsNoSourceFiles()
        {
            var results = CreateChutzpahJsonParser().GenerateSourceFiles(CHUTZPAH_JSON_EXMAPLE, false);

            Assert.Equal(2, results.Count);
            Assert.Equal(36, results[0].Coverage[0]);
            Assert.Equal(10, results[1].Coverage[5]);
            Assert.Equal(null, results[0].Coverage[7]);
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


        private ChutzpahJsonParser CreateChutzpahJsonParser()
        {
            return new ChutzpahJsonParser(null);
        }
    }
}
