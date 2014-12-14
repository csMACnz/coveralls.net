namespace csmacnz.Coveralls
{
    public interface IFileSystem
    {
        string TryLoadFile(string filePath);
    }
}