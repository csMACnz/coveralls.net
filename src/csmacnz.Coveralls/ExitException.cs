namespace csmacnz.Coveralls;

#pragma warning disable S3925 // "ISerializable" should be implemented correctly
public class ExitException : Exception
#pragma warning restore S3925 // "ISerializable" should be implemented correctly
{
    public ExitException(string message)
        : base(message)
    {
    }

    public ExitException()
    {
    }

    public ExitException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
