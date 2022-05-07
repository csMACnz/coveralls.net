namespace csmacnz.Coveralls.Adapters;

public class StandardConsole : IConsole
{
    public void WriteLine(string message)
        => Console.WriteLine(message);

    public void WriteErrorLine(string message)
        => Console.Error.WriteLine(message);
}
