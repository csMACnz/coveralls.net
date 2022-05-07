namespace csmacnz.Coveralls.Adapters;

public class FileSystem : IFileSystem
{
    public Option<string> TryLoadFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            return File.ReadAllText(filePath);
        }

        return Option<string>.None;
    }

    public Option<FileInfo[]> GetFiles(string directory)
    {
        if (Directory.Exists(directory))
        {
            return new DirectoryInfo(directory).GetFiles();
        }

        return Option<FileInfo[]>.None;
    }

    public bool WriteFile(string outputFile, string fileData)
    {
        try
        {
            File.WriteAllText(outputFile, fileData);
        }
        catch (Exception)
        {
            // Maybe should give reason.
            return false;
        }

        return true;
    }

    public Option<string[]> TryReadAllLinesFromFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            return File.ReadAllLines(filePath);
        }

        return Option<string[]>.None;
    }
}
