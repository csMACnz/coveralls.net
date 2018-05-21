using Beefeater;

namespace csmacnz.Coveralls.MetaDataResolvers
{
    public interface IMetaDataResolver
    {
        bool IsActive();

        Option<string> ResolveServiceName();

        Option<string> ResolveServiceJobId();

        Option<string> ResolveServiceNumber();

        Option<string> ResolvePullRequestId();
    }
}
