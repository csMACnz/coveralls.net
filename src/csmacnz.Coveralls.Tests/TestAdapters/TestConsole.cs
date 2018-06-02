using System.Collections.Generic;
using csmacnz.Coveralls.Ports;

namespace csmacnz.Coveralls.Tests.TestAdapters
{
    public class TestConsole : IConsole
    {
        private readonly List<string> _errors = new List<string>();
        private readonly List<string> _standardOut = new List<string>();

        public void WriteLine(string message)
        {
            _standardOut.Add(message);
        }

        public void WriteErrorLine(string message)
        {
            _errors.Add(message);
        }

        public string[] Errors => _errors.ToArray();

        public string[] StandardOut => _standardOut.ToArray();
    }
}
