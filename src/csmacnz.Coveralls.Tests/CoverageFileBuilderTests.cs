using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csmacnz.Coveralls.Tests
{
    [TestClass]
    public class CoverageFileBuilderTests
    {
        [TestMethod]
        public void CanConstructBuilder()
        {
            var builder = CreateFileBuilder();
            Assert.IsNotNull(builder);
        }

        [TestMethod]
        public void NewBuilderCanCreateCoverageFile()
        {
            var builder = CreateFileBuilder();
            var coverageFile = builder.CreateFile();
            Assert.IsNotNull(coverageFile);
        }

        [TestClass]
        public class WhenCreatingACoverageFromADefaultBuilder
        {
            private CoverageFile _coverageFile;
            private const string FILENAME = @"C:\sourceFile.cs";

            [TestInitialize]
            public void Setup()
            {
                var builder = CoverageFileBuilderTests.CreateFileBuilder(FILENAME);
                _coverageFile = builder.CreateFile();
            }

            [TestMethod]
            public void ThenFileNameIsSet()
            {
                Assert.AreEqual(FILENAME, _coverageFile.Name);
            }
            
            [TestMethod]
            public void ThenSourceIsEmpty()
            {
                Assert.AreEqual(string.Empty, _coverageFile.Source);
            }
            
            [TestMethod]
            public void ThenCoverageIsEmpty()
            {
                Assert.IsNotNull(_coverageFile.Coverage);
                Assert.AreEqual(1, _coverageFile.Coverage.Length);
                Assert.IsNull(_coverageFile.Coverage[0]);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NewBuilderWithInvalidPathThrowsArgumentException()
        {
            var builder = new CoverageFileBuilder("");
        }

        public static CoverageFileBuilder CreateFileBuilder(string filePath = @"C:\temp\file.cs")
        {
            return new CoverageFileBuilder(filePath);
        }
    }
}
