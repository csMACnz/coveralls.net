namespace csmacnz.Coveralls
{
    public class AppVeyorGitDataResolver : IGitDataResolver
    {
        private readonly IEnvironmentVariables _variables;

        public AppVeyorGitDataResolver(IEnvironmentVariables variables)
        {
            _variables = variables;
        }

        public bool CanProvideData()
        {
            return _variables.GetEnvironmentVariable("APPVEYOR") == "True";
        }

        public GitData GenerateData()
        {
            throw new System.NotImplementedException();
        }
    }
}