using System.IO;

namespace csmacnz.Coveralls.Tests.Integration
{
    public class RepositoryPaths
    {
        public static string GetSamplesPath()
        {
            return Path.Combine("..", "..", "..", "..", "..", "CoverageSamples");
        }
    }
}