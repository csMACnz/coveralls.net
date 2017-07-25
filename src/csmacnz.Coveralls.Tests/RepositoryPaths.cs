using System.IO;

namespace csmacnz.Coveralls.Tests
{
    public class RepositoryPaths
    {
        public static string GetSamplesPath()
        {
            return Path.Combine("..", "..", "..", "..", "..", "CoverageSamples");
        }
    }
}