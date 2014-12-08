using Mono.Options;
using System;

namespace csmacnz.Coveralls
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var p = new OptionSet();
            p.Parse(args);

            Console.WriteLine("Hello World");
            Console.ReadKey();
        }
    }
}
