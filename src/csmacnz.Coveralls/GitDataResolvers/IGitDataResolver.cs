using csmacnz.Coveralls.Data;

namespace csmacnz.Coveralls.GitDataResolvers
{
    public interface IGitDataResolver
    {
        bool CanProvideData();

        GitData GenerateData();
    }
}