using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using csmacnz.Coveralls;

namespace csmacnz.Coveralls.Tests
{
    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        public void CanExecuteMain()
        {
            Program.Main(new[] { "Coveralls.exe" });
        }
    }
}
