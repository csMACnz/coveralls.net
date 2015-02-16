using System.Collections.Generic;
using DocoptNet;

namespace csmacnz.Coveralls
{
    // Generated class for Main.usage.txt
    public class MainArgs
    {
        public const string Usage = @"csmac.Coveralls - a coveralls.io coverage publisher for .Net

Usage:
  csmacnz.Coveralls (--opencover | --vscodecoverage | --monocov) -i ./opencovertests.xml --repoToken <repoToken> [-o ./opencovertests.json] [--dryrun] [--useRelativePaths --basePath <path>] [--commitId <commitId> --commitBranch <commitBranch> [--commitAuthor <commitAuthor> --commitEmail <commitEmail> --commitMessage <commitMessage>] ] [--jobId <jobId>] [--serviceName <Name>]
  csmacnz.Coveralls --version
  csmacnz.Coveralls --help

Options:
 -h, --help                      Show this screen.
 --version                       Show version.
 -i <file>, --input <file>       The coverage source file location.
 -o <file>, --output <file>      The coverage results json will be written to this file it provided. 
 --dryrun                        This flag will stop coverage results being posted to coveralls.io
 --useRelativePaths              This flag, when provided, will attempt to strip the current working directory from the beginning of the source file path.
 --basePath <path>               When useRelativePaths and a basePath is provided, this path is used instead of the current working directory.
 --opencover                     Reads input as OpenCover data.
 --vscodecoverage                Reads input as the CodeCoverage.exe xml.
 --monocov                       Reads input as monocov results folder.
 --repoToken <repoToken>         The coveralls.io repository token.
 --commitId <commitId>           The git commit hash for the coverage report.
 --commitAuthor <commitAuthor>   The git commit author for the coverage report.
 --commitEmail <commitEmail>     The git commit author email for the coverage report.
 --commitMessage <commitMessage> The git commit message for the coverage report.
 --commitBranch <commitBranch>   The git branch for the coverage report.
 --jobId <jobId>                 The job Id to provide to coveralls.io.
 --serviceName <Name>            The service-name for the coverage report.

What its for:
 Reads your .Net code coverage output data and submits it to
 coveralls.io's service. This can be used by your build scripts 
 or with a CI builder server.";
        private readonly IDictionary<string, ValueObject> _args;

        public MainArgs(
            ICollection<string> argv,
            bool help = true,
            object version = null,
            bool optionsFirst = false,
            bool exit = false)
        {
            _args = new Docopt().Apply(Usage, argv, help, version, optionsFirst, exit);
        }

        public IDictionary<string, ValueObject> Args
        {
            get { return _args; }
        }

        public bool IsProvided(string parameter)
        {
            return _args[parameter] != null;
        }

		public bool OptOpencover { get { return _args["--opencover"].IsTrue; } }
		public bool OptVscodecoverage { get { return _args["--vscodecoverage"].IsTrue; } }
		public bool OptMonocov { get { return _args["--monocov"].IsTrue; } }
		public string OptInput { get { return _args["--input"].ToString(); } }
		public string OptRepotoken { get { return _args["--repoToken"].ToString(); } }
		public string OptOutput { get { return _args["--output"].ToString(); } }
		public bool OptDryrun { get { return _args["--dryrun"].IsTrue; } }
		public bool OptUserelativepaths { get { return _args["--useRelativePaths"].IsTrue; } }
		public string OptBasepath { get { return _args["--basePath"].ToString(); } }
		public string OptCommitid { get { return _args["--commitId"].ToString(); } }
		public string OptCommitbranch { get { return _args["--commitBranch"].ToString(); } }
		public string OptCommitauthor { get { return _args["--commitAuthor"].ToString(); } }
		public string OptCommitemail { get { return _args["--commitEmail"].ToString(); } }
		public string OptCommitmessage { get { return _args["--commitMessage"].ToString(); } }
		public string OptJobid { get { return _args["--jobId"].ToString(); } }
		public string OptServicename { get { return _args["--serviceName"].ToString(); } }
		public bool OptVersion { get { return _args["--version"].IsTrue; } }
		public bool OptHelp { get { return _args["--help"].IsTrue; } }
    }
}
