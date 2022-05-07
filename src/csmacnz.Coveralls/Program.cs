namespace csmacnz.Coveralls;

public class Program
{
    private readonly IConsole _console;
    private readonly string _version;
    private readonly IFileSystem _fileSystem;
    private readonly IEnvironmentVariables _environmentVariables;
    private readonly ICoverallsService _coverallsService;

    // NOTE(markc): make this configurable, especially if private servers are used.
    private static readonly Uri BaseUri = new("https://coveralls.io");

    public Program(
        IConsole console,
        IFileSystem fileSystem,
        IEnvironmentVariables environmentVariables,
        ICoverallsService coverallsService,
        string version)
    {
        _console = console;
        _fileSystem = fileSystem;
        _environmentVariables = environmentVariables;
        _coverallsService = coverallsService;
        _version = version;
    }

    public static void Main(string[] argv)
    {
        var console = new StandardConsole();

        var result = new Program(console, new FileSystem(), new EnvironmentVariables(), new CoverallsService(BaseUri), GetDisplayVersion()).Run(argv);
        if (result.HasValue)
        {
            Environment.Exit(result.Value);
        }
    }

    private static NotNull<string> GetDisplayVersion()
        => Assembly
            .GetEntryAssembly()!
            .GetCustomAttribute<AssemblyFileVersionAttribute>()!
            .Version;

    public int? Run(string[] argv)
    {
        try
        {
            var args = new MainArgs(argv, version: _version);
            if (args.Failed)
            {
                _console.WriteLine(args.FailMessage!);
                return args.FailErrorCode;
            }

            var metadata = CoverageMetadataResolver.Resolve(args, _environmentVariables);

            if (args.OptCompleteParallelWork)
            {
                var repoToken = ResolveRepoToken(args);

                var pushResult = _coverallsService.PushParallelCompleteWebhook(repoToken, metadata.ServiceBuildNumber);
                if (!pushResult.Successful)
                {
                    ExitWithError(pushResult.Error);
                }

                return null;
            }

            var settings = LoadSettings(args);

            var gitData = ResolveGitData(_console, args);

            var app = new CoverallsPublisher(_console, _fileSystem, _coverallsService);
            var result = app.Run(settings, gitData, metadata);
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

    private Either<GitData, CommitSha>? ResolveGitData(IConsole console, MainArgs args)
    {
        var providers = new List<IGitDataResolver>
        {
            new CommandLineGitDataResolver(args),
            new AppVeyorGitDataResolver(_environmentVariables),
            new TeamCityGitDataResolver(_environmentVariables, _console)
        };

        var provider = providers.FirstOrDefault(p => p.CanProvideData());
        if (provider is null)
        {
            console.WriteLine("No git data available");
        }
        else
        {
            console.WriteLine($"Using Git Data Provider '{provider.DisplayName}'");
            var data = provider.GenerateData();
            if (data.HasValue)
            {
                return data;
            }
        }

        return null;
    }

    private ConfigurationSettings LoadSettings(MainArgs args)
    {
        var settings = new ConfigurationSettings(
            RepoToken: ResolveRepoToken(args),
            OutputFile: args.IsProvided("--output") ? args.OptOutput : null,
            DryRun: args.OptDryrun,
            TreatUploadErrorsAsWarnings: args.OptTreatuploaderrorsaswarnings,
            UseRelativePaths: args.OptUserelativepaths,
            BasePath: args.IsProvided("--basePath") ? args.OptBasepath : null,
            ParseCoverageSources(args));
        return settings;
    }

    private string ResolveRepoToken(MainArgs args)
    {
        string? repoToken = null;
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
            else
            {
                repoToken = _environmentVariables.GetEnvironmentVariable(variable);
            }

            if (repoToken.IsNullOrWhitespace())
            {
                ExitWithError($"No token found in Environment Variable '{variable}'.");
            }
        }

        return repoToken;
    }

    private static List<CoverageSource> ParseCoverageSources(MainArgs args)
    {
        var optInput = args.OptInput ?? throw new ArgumentException(
            message: "Parsing Sources mode requires input",
            paramName: nameof(args));

        List<CoverageSource> results = new();
        if (args.OptMultiple)
        {
            var modes = optInput.Split(';');
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
                else
                {
                    results.Add(new CoverageSource(mode.Value, input));
                }
            }
        }
        else
        {
            var mode = GetMode(args);
            if (!mode.HasValue)
            {
                ExitWithError("Unknown mode provided");
            }
            else
            {
                results.Add(new CoverageSource(mode.Value, optInput));
            }
        }

        return results;
    }

    private static CoverageMode? GetMode(string mode)
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

        return null;
    }

    private static CoverageMode? GetMode(MainArgs args)
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

        return null; // Unreachable
    }

    [ContractAnnotation("=>halt")]
    [System.Diagnostics.CodeAnalysis.DoesNotReturn]
    private static void ExitWithError(string message) => throw new ExitException(message);
}
