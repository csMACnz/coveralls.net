using System.IO;

namespace csmacnz.Coveralls.Tests.Integration;

public static class RepositoryPaths
{
    public static string GetSamplesPath() => Path.Combine("..", "..", "..", "..", "..", "src", "csmacnz.Coveralls.Tests", "Reports");
}
