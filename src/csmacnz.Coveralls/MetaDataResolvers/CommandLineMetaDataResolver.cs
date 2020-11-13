using Beefeater;

namespace csmacnz.Coveralls.MetaDataResolvers
{
    public class CommandLineMetaDataResolver : IMetaDataResolver
    {
        private readonly MainArgs _args;

        public CommandLineMetaDataResolver(MainArgs args)
        {
            _args = args;
        }

        public bool IsActive()
        {
            return true;
        }

        public Option<string> ResolveServiceName()
        {
            if (_args.IsProvided("--serviceName"))
            {
                return _args.OptServicename ?? string.Empty;
            }

            return Option<string>.None;
        }

        public Option<string> ResolveServiceJobId()
        {
            if (_args.IsProvided("--jobId"))
            {
                return _args.OptJobid ?? string.Empty;
            }

            return Option<string>.None;
        }

        public Option<string> ResolveServiceBuildNumber()
        {
            if (_args.IsProvided("--serviceNumber"))
            {
                return _args.OptServicenumber ?? string.Empty;
            }

            return Option<string>.None;
        }

        public Option<string> ResolvePullRequestId()
        {
            if (_args.IsProvided("--pullRequest"))
            {
                return _args.OptPullrequest ?? string.Empty;
            }

            return Option<string>.None;
        }
    }
}
