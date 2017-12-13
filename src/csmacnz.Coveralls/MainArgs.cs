using System.Collections.Generic;
using DocoptNet;

namespace csmacnz.Coveralls
{
    public class MainArgs
    {
        public const string Usage = @"csmacnz.Coveralls - a coveralls.io coverage publisher for .Net

Usage:
  csmacnz.Coveralls (--opencover | --dynamiccodecoverage | --monocov | --exportcodecoverage | --chutzpah | --lcov | --ncover | --multiple) -i ./opencovertests.xml (--repoToken <repoToken> | [--repoTokenVariable <repoTokenVariable>]) [-o ./opencovertests.json] [--dryrun] [--useRelativePaths [--basePath <path>] ] [--commitId <commitId> --commitBranch <commitBranch> [--commitAuthor <commitAuthor> --commitEmail <commitEmail> --commitMessage <commitMessage>] ] [--jobId <jobId>] [--serviceName <Name>] [--serviceNumber <Number>] [--pullRequest <pullRequestId>] [--treatUploadErrorsAsWarnings] [--parallel]
  csmacnz.Coveralls --version
  csmacnz.Coveralls --help

Options:
 -h, --help                               Show this screen.
 --version                                Show version.
 -i <file>, --input <file>                The coverage source file location. When --aggregate is used, this is a collection of inputs to publish together. The format is ""monocov=coverage.cov;chutzpah=chutzpah.json""
 -o <file>, --output <file>               The coverage results json will be written to this file if provided.
 --dryrun                                 This flag will stop coverage results being posted to coveralls.io
 --useRelativePaths                       This flag, when provided, will attempt to strip the current working directory from the beginning of the source file path.
 --basePath <path>                        When useRelativePaths and a basePath is provided, this path is used instead of the current working directory.
 --opencover                              Reads input as OpenCover data.
 --dynamiccodecoverage                    Reads input as the CodeCoverage.exe xml format.
 --exportcodecoverage                     Reads input as the Visual Studio Coverage Export xml format
 --monocov                                Reads input as monocov results folder.
 --chutzpah                               Reads input as chutzpah json data.
 --lcov									  Reads input as lcov format.
 --ncover								  Reads input as NCover format.
 --multiple                               Provide multiple types and files. This mode requires -i to provide values in the format ""chutzpah=chutzpahFile.json;opencover=opencoverFile.xml""
 --parallel                               If using the parallel builds. If sent, it will wait for the webhook before completing the build.
 --repoToken <repoToken>                  The coveralls.io repository token.
 --repoTokenVariable <repoTokenVariable>  The Environment Variable name where the coveralls.io repository token is available. [default: COVERALLS_REPO_TOKEN]
 --commitId <commitId>                    The git commit hash for the coverage report.
 --commitBranch <commitBranch>            The git branch for the coverage report.
 --commitAuthor <commitAuthor>            The git commit author for the coverage report.
 --commitEmail <commitEmail>              The git commit author email for the coverage report.
 --commitMessage <commitMessage>          The git commit message for the coverage report.
 --jobId <jobId>                          The job Id to provide to coveralls.io. [default: 0]
 --serviceName <Name>                     The service-name for the coverage report. [default: coveralls.net]
 --serviceNumber <Number>                 The service-number for the coverage report.
 --pullRequest <pullRequestId>            The github pull request id. Used for updating status on github PRs.
 -k, --treatUploadErrorsAsWarnings        Exit successfully if an upload error is encountered and this flag is set.

Commit Options:
  If --commitId and --commitBranch are provided, all git settings will come from the command line arguments.
  If you do not provide any commit information, the correct information will be resolved for you using the following logic:
    * If you are on AppVeyor build server ($env:APPVEYOR -eq $true) We will locate the data from the Environment Variables.
    * If git is available in the current working directory, the settings will be loaded from git.


What it's for:
 Reads your .Net code coverage output data and submits it to
 coveralls.io's service. This can be used by your build scripts
 or with a CI builder server.";

        private readonly IDictionary<string, ValueObject> _args;

        public MainArgs(
            ICollection<string> argv,
            bool help = true,
            object version = null,
            bool optionsFirst = false)
        {
            var docOpt = new Docopt();
            docOpt.PrintExit += (sender, args) =>
            {
                Failed = true;
                FailMessage = args.Message;
                FailErrorCode = args.ErrorCode;
            };
            _args = docOpt.Apply(Usage, argv, help, version, optionsFirst, true);
        }

        public bool Failed { get; private set; }

        public string FailMessage { get; private set; }

        public int FailErrorCode { get; private set; }

        public IDictionary<string, ValueObject> Args => _args;

        public bool IsProvided(string parameter)
        {
            return _args[parameter] != null;
        }

        public bool OptOpencover => _args["--opencover"].IsTrue;

        public bool OptDynamiccodecoverage => _args["--dynamiccodecoverage"].IsTrue;

        public bool OptMonocov => _args["--monocov"].IsTrue;

        public bool OptExportcodecoverage => _args["--exportcodecoverage"].IsTrue;

        public bool OptChutzpah => _args["--chutzpah"].IsTrue;

        public bool OptLcov => _args["--lcov"].IsTrue;

        public bool OptNCover => _args["--ncover"].IsTrue;

        public bool OptMultiple => _args["--multiple"].IsTrue;

        public string OptInput => _args["--input"]?.ToString();

        public string OptRepotoken => _args["--repoToken"]?.ToString();

        public string OptRepotokenvariable => _args["--repoTokenVariable"]?.ToString();

        public string OptOutput => _args["--output"]?.ToString();

        public bool OptDryrun => _args["--dryrun"].IsTrue;

        public bool OptUserelativepaths => _args["--useRelativePaths"].IsTrue;

        public string OptBasepath => _args["--basePath"]?.ToString();

        public string OptCommitid => _args["--commitId"]?.ToString();

        public string OptCommitbranch => _args["--commitBranch"]?.ToString();

        public string OptCommitauthor => _args["--commitAuthor"]?.ToString();

        public string OptCommitemail => _args["--commitEmail"]?.ToString();

        public string OptCommitmessage => _args["--commitMessage"]?.ToString();

        public string OptJobid => _args["--jobId"]?.ToString();

        public string OptServicename => _args["--serviceName"]?.ToString();

        public string OptServicenumber => _args["--serviceNumber"]?.ToString();

        public string OptPullrequest => _args["--pullRequest"]?.ToString();

        public bool OptTreatuploaderrorsaswarnings => _args["--treatUploadErrorsAsWarnings"].IsTrue;

        public bool OptParallel => _args["--parallel"].IsTrue;

        public bool OptVersion => _args["--version"].IsTrue;

        public bool OptHelp => _args["--help"].IsTrue;
    }
}
