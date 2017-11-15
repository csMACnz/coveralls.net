using System;

namespace csmacnz.Coveralls
{
    public class ExitException : Exception
    {
        public ExitException(string message)
            : base(message)
        {
        }
    }
}
