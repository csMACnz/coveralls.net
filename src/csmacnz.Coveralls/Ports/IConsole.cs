namespace csmacnz.Coveralls.Ports;

public interface IConsole
{
    void WriteLine(string message);

    void WriteErrorLine(string message);
}
