namespace csmacnz.Coveralls.Data
{
    public class CommitSha
    {
        private readonly string _value;

        public CommitSha(string value)
        {
            _value = value;
        }

        public string Value => _value;
    }
}
