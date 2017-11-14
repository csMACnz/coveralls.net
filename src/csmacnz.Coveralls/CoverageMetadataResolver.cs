using BCLExtensions;
using Beefeater;
using csmacnz.Coveralls.Adapters;

namespace csmacnz.Coveralls
{
    public static class CoverageMetadataResolver
    {
        public static CoverageMetadata Resolve(MainArgs args)
        {
            var serviceName = ResolveServiceName(args);
            var serviceJobId = ResolveServiceJobId(args);
            var serviceNumber = ResolveServiceNumber(args);
            var pullRequestId = ResolvePullRequestId(args);
            var parallel = args.IsProvided("--parallel") && args.OptParallel;

            return new CoverageMetadata
            {
                ServiceJobId = serviceJobId.ValueOr("0"),
                ServiceName = serviceName.ValueOr("coveralls.net"),
                ServiceNumber = serviceNumber.ValueOr(null),
                PullRequestId = pullRequestId.ValueOr(null),
                Parallel = parallel
            };
        }


        private static Option<string> ResolveServiceName(MainArgs args)
        {
            if (args.IsProvided("--serviceName")) return args.OptServicename;
            var isAppVeyor = new EnvironmentVariables().GetEnvironmentVariable("APPVEYOR");
            if (isAppVeyor == "True") return "appveyor";
            return null;
        }

        private static Option<string> ResolveServiceJobId(MainArgs args)
        {
            if (args.IsProvided("--jobId")) return args.OptJobid;
            var jobId = new EnvironmentVariables().GetEnvironmentVariable("APPVEYOR_JOB_ID");
            if (jobId.IsNotNullOrWhitespace()) return jobId;
            return null;
        }

        private static Option<string> ResolveServiceNumber(MainArgs args)
        {
            if (args.IsProvided("--serviceNumber")) return args.OptServicenumber;
            var jobId = new EnvironmentVariables().GetEnvironmentVariable("APPVEYOR_BUILD_NUMBER");
            if (jobId.IsNotNullOrWhitespace()) return jobId;
            return null;
        }

        private static Option<string> ResolvePullRequestId(MainArgs args)
        {
            if (args.IsProvided("--pullRequest")) return args.OptPullrequest;
            var prId = new EnvironmentVariables().GetEnvironmentVariable("APPVEYOR_PULL_REQUEST_NUMBER");
            if (prId.IsNotNullOrWhitespace()) return prId;
            return null;
        }

    }
}