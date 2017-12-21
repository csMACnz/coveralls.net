using System.IO;

namespace csmacnz.Coveralls.Tests.TestHelpers
{
    public static class RepositoryPaths
    {
        public static string GetSamplesPath()
        {
            return Path.Combine("..", "..", "..", "..", "..", "CoverageSamples");
        }
    }
}
