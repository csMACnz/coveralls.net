using System.Collections.Generic;
using System.Xml.Linq;
using csmacnz.Coveralls.Data;

namespace csmacnz.Coveralls.Parsers
{
    internal interface IXmlCoverageParser
    {
        List<FileCoverageData> GenerateSourceFiles(XDocument document);
    }
}