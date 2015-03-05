namespace csmacnz.Coveralls
{
    public class AppVeyorGitDataResolver : IGitDataResolver
    {
        public bool CanProvideData()
        {
            return false;
        }

        public GitData GenerateData()
        {
            throw new System.NotImplementedException();
        }
    }
}