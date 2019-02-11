﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BCLExtensions;
using Beefeater;
using csmacnz.Coveralls.Adapters;
using csmacnz.Coveralls.Data;
using csmacnz.Coveralls.GitDataResolvers;
using csmacnz.Coveralls.Ports;
using JetBrains.Annotations;

namespace csmacnz.Coveralls
{
    public class Program
    {
        private readonly IConsole _console;
        private readonly string _version;
        private readonly IFileSystem _fileSystem;

        public Program(IConsole console, IFileSystem fileSystem, string version)
        {
            _console = console;
            _fileSystem = fileSystem;
            _version = version;
        }

        public static void Main(string[] argv)
        {
            var console = new StandardConsole();
            var result = new Program(console, new FileSystem(), GetDisplayVersion()).Run(argv);
            if (result.HasValue)
            {
                Environment.Exit(result.Value);
            }
        }

        private static NotNull<string> GetDisplayVersion()
        {
            return Assembly
                .GetEntryAssembly()
                .GetCustomAttribute<AssemblyFileVersionAttribute>()
                .Version;
        }

        public int? Run(string[] argv)
        {
            try
            {
                var args = new MainArgs(argv, version: _version);
                if (args.Failed)
                {
                    _console.WriteLine(args.FailMessage);
                    return args.FailErrorCode;
                }

                var gitData = ResolveGitData(_console, args);

                var settings = LoadSettings(args);

                var metadata = CoverageMetadataResolver.Resolve(args);

                var app = new CoverallsPublisher(_console, _fileSystem);
                var result = app.Run(settings, gitData.ValueOrDefault(), metadata);
                if (!result.Successful)
                {
                    ExitWithError(result.Error);
                }

                return null;
            }
            catch (ExitException ex)
            {
                _console.WriteErrorLine(ex.Message);
                return 1;
            }
        }

        private static Option<GitData> ResolveGitData(IConsole console, MainArgs args)
        {
            var currentEnvironment = new EnvironmentVariables();
            var providers = new List<IGitDataResolver>
            {
                new CommandLineGitDataResolver(args),
                new AppVeyorGitDataResolver(currentEnvironment),
                new GitlabGitDataResolver(currentEnvironment)
            };

            var provider = providers.FirstOrDefault(p => p.CanProvideData());
            if (provider is null)
            {
                console.WriteLine("No git data available");
                return Option<GitData>.None;
            }

            console.WriteLine($"Using Git Data Provider '{provider.DisplayName}'");
            return provider.GenerateData();
        }

        private static ConfigurationSettings LoadSettings(MainArgs args)
        {
            var settings = new ConfigurationSettings
            {
                RepoToken = ResolveRepoToken(args),
                OutputFile = args.IsProvided("--output") ? args.OptOutput : string.Empty,
                DryRun = args.OptDryrun,
                TreatUploadErrorsAsWarnings = args.OptTreatuploaderrorsaswarnings,
                UseRelativePaths = args.OptUserelativepaths,
                BasePath = args.IsProvided("--basePath") ? args.OptBasepath : null
            };
            settings.CoverageSources.AddRange(ParseCoverageSources(args));
            return settings;
        }

        private static string ResolveRepoToken(MainArgs args)
        {
            string repoToken;
            if (args.IsProvided("--repoToken"))
            {
                repoToken = args.OptRepotoken;
                if (repoToken.IsNullOrWhitespace())
                {
                    ExitWithError("parameter repoToken is required.");
                }
            }
            else
            {
                var variable = args.OptRepotokenvariable;
                if (variable.IsNullOrWhitespace())
                {
                    ExitWithError("parameter repoTokenVariable is required.");
                }

                repoToken = Environment.GetEnvironmentVariable(variable);
                if (repoToken.IsNullOrWhitespace())
                {
                    ExitWithError("No token found in Environment Variable '{0}'.".FormatWith(variable));
                }
            }

            return repoToken;
        }

        private static List<CoverageSource> ParseCoverageSources(MainArgs args)
        {
            List<CoverageSource> results = new List<CoverageSource>();
            if (args.OptMultiple)
            {
                var modes = args.OptInput.Split(';');
                foreach (var modekeyvalue in modes)
                {
                    var split = modekeyvalue.Split('=');
                    var rawMode = split[0];
                    var input = split[1];
                    var mode = GetMode(rawMode);
                    if (!mode.HasValue)
                    {
                        ExitWithError("Unknown mode provided");
                    }

                    results.Add(new CoverageSource((CoverageMode)mode, input));
                }
            }
            else
            {
                var mode = GetMode(args);
                if (!mode.HasValue)
                {
                    ExitWithError("Unknown mode provided");
                }

                results.Add(new CoverageSource((CoverageMode)mode, args.OptInput));
            }

            return results;
        }

        private static Option<CoverageMode> GetMode(string mode)
        {
            if (string.Equals(mode, "monocov", StringComparison.OrdinalIgnoreCase))
            {
                return CoverageMode.MonoCov;
            }

            if (string.Equals(mode, "chutzpah", StringComparison.OrdinalIgnoreCase))
            {
                return CoverageMode.Chutzpah;
            }

            if (string.Equals(mode, "dynamiccodecoverage", StringComparison.OrdinalIgnoreCase))
            {
                return CoverageMode.DynamicCodeCoverage;
            }

            if (string.Equals(mode, "exportcodecoverage", StringComparison.OrdinalIgnoreCase))
            {
                return CoverageMode.ExportCodeCoverage;
            }

            if (string.Equals(mode, "opencover", StringComparison.OrdinalIgnoreCase))
            {
                return CoverageMode.OpenCover;
            }

            if (string.Equals(mode, "lcov", StringComparison.OrdinalIgnoreCase))
            {
                return CoverageMode.LCov;
            }

            if (string.Equals(mode, "ncover", StringComparison.OrdinalIgnoreCase))
            {
                return CoverageMode.NCover;
            }

            if (string.Equals(mode, "reportgenerator", StringComparison.OrdinalIgnoreCase))
            {
                return CoverageMode.ReportGenerator;
            }

            return Option<CoverageMode>.None;
        }

        private static Option<CoverageMode> GetMode(MainArgs args)
        {
            if (args.OptMonocov)
            {
                return CoverageMode.MonoCov;
            }

            if (args.OptChutzpah)
            {
                return CoverageMode.Chutzpah;
            }

            if (args.OptDynamiccodecoverage)
            {
                return CoverageMode.DynamicCodeCoverage;
            }

            if (args.OptExportcodecoverage)
            {
                return CoverageMode.ExportCodeCoverage;
            }

            if (args.OptOpencover)
            {
                return CoverageMode.OpenCover;
            }

            if (args.OptLcov)
            {
                return CoverageMode.LCov;
            }

            if (args.OptNCover)
            {
                return CoverageMode.NCover;
            }

            if (args.OptReportGenerator)
            {
                return CoverageMode.ReportGenerator;
            }

            return Option<CoverageMode>.None; // Unreachable
        }

        [ContractAnnotation("=>halt")]
        private static void ExitWithError(string message)
        {
            throw new ExitException(message);
        }
    }
}
