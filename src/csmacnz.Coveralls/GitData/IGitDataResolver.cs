namespace csmacnz.Coveralls
{
    public interface IGitDataResolver
    {
        bool CanProvideData();
        GitData GenerateData();
    }
}