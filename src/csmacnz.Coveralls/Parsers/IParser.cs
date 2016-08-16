using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csmacnz.Coveralls.Parsers
{
    public interface IParser
    {
        List<CoverageFile> GenerateSourceFiles(string filePath, bool useRelativePaths);
    }
}
