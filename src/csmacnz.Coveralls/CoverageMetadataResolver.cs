﻿using System;
using System.Collections.Generic;
using System.Linq;
using Beefeater;
using csmacnz.Coveralls.MetaDataResolvers;
using csmacnz.Coveralls.Ports;

namespace csmacnz.Coveralls
{
    public static class CoverageMetadataResolver
    {
        public static CoverageMetadata Resolve(MainArgs args, IEnvironmentVariables variables)
        {
            _ = args ?? throw new ArgumentNullException(paramName: nameof(args));

            var resolvers = CreateResolvers(args, variables);
            var serviceName = Resolve(resolvers, r => r.ResolveServiceName());
            var serviceJobId = Resolve(resolvers, r => r.ResolveServiceJobId());
            var serviceBuildNumber = Resolve(resolvers, r => r.ResolveServiceBuildNumber());
            var pullRequestId = Resolve(resolvers, r => r.ResolvePullRequestId());
            var parallel = ResolveParallel(args, variables);

            return new CoverageMetadata(
                serviceName: serviceName.ValueOr("coveralls.net"),
                serviceJobId: serviceJobId.ValueOr("0"),
                serviceBuildNumber: serviceBuildNumber.ValueOrDefault(),
                pullRequestId: pullRequestId.ValueOrDefault(),
                parallel: parallel);
        }

        private static List<IMetaDataResolver> CreateResolvers(MainArgs args, IEnvironmentVariables variables)
        {
            return new List<IMetaDataResolver>
            {
                new CommandLineMetaDataResolver(args),
                new AppVeyorMetaDataResolver(variables),
                new TravisMetaDataResolver(variables),
                new TeamCityMetaDataResolver(variables)
            };
        }

        private static Option<string> Resolve(List<IMetaDataResolver> resolvers, Func<IMetaDataResolver, Option<string>> resolve)
        {
            return resolvers
                .Where(r => r.IsActive())
                .Select(r => resolve?.Invoke(r) ?? Option<string>.None)
                .FirstOrDefault(v => v.HasValue) !;
        }

        private static bool ResolveParallel(MainArgs args, IEnvironmentVariables variables)
        {
            if (args.IsProvided("--parallel"))
            {
                return args.OptParallel;
            }

            return variables.GetBooleanVariable("COVERALLS_PARALLEL");
        }
    }
}
