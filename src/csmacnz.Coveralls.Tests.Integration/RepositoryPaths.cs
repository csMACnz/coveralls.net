using System.IO;

namespace csmacnz.Coveralls.Tests.Integration
{
    public static class RepositoryPaths
    {
        public static string GetSamplesPath()
        {
            return Path.Combine("..", "..", "..", "..", "..", "src", "csmacnz.Coveralls.Tests", "Reports");
        }
    }
}
