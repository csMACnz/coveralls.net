using System.IO;

namespace csmacnz.Coveralls.Tests
{
    public static class RepositoryPaths
    {
        public static string GetSamplesPath()
        {
            return Path.Combine("..", "..", "..", "..", "..", "CoverageSamples");
        }
    }
}