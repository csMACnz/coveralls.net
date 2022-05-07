namespace csmacnz.Coveralls.GitDataResolvers;

public interface IGitDataResolver
{
    bool CanProvideData();

    Either<GitData, CommitSha>? GenerateData();

    string DisplayName { get; }
}
