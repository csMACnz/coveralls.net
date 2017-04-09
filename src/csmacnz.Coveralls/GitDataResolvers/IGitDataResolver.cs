namespace csmacnz.Coveralls.GitDataResolvers
{
    public interface IGitDataResolver
    {
        bool CanProvideData();
        GitData GenerateData();
    }
}