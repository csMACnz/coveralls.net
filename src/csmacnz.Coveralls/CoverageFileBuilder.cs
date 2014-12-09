using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csmacnz.Coveralls
{
    public class CoverageFileBuilder
    {
        private string _filePath;
        
        public CoverageFileBuilder(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("filePath");
            }
            _filePath = filePath;
        }

        public CoverageFile CreateFile()
        {
            return new CoverageFile(_filePath, new string[] { "" }, new int?[] { null });
        }
    }
}
