namespace csmacnz.Coveralls;

public class PathProcessor
{
    private readonly string _basePath;

    public PathProcessor(string? basePath) => _basePath = basePath.IsNotNullOrWhitespace() ? basePath! : Directory.GetCurrentDirectory();

    public string ConvertPath(string path)
    {
        _ = path ?? throw new ArgumentNullException(nameof(path));

        var currentWorkingDirectory = _basePath.ToLower(CultureInfo.InvariantCulture);

        if (path.ToLower(CultureInfo.InvariantCulture).StartsWith(currentWorkingDirectory, StringComparison.InvariantCulture))
        {
            return path[currentWorkingDirectory.Length..];
        }

        return path;
    }

    public static string UnixifyPath(string filePath)
    {
        _ = filePath ?? throw new ArgumentNullException(nameof(filePath));

        return filePath.Replace("\\", "/", StringComparison.InvariantCulture).Replace(":", string.Empty, StringComparison.InvariantCulture);
    }
}
