using System.Text.Json;

namespace TypedRest.CommandLine.IO;

/// <summary>
/// Reads from and writes to the default system console (stdin/stdout/stderr) using JSON serialization for objects.
/// </summary>
public class JsonConsole : IConsole
{
    public T? Read<T>()
        => JsonSerializer.Deserialize<T>(Console.ReadLine() ?? throw new EndOfStreamException());

    public string Read(string prompt)
    {
        Console.Write(prompt + " ");
        return Console.ReadLine() ?? throw new EndOfStreamException();
    }

    public string ReadSecret(string prompt)
    {
        // Increase maximum length for Console.ReadLine()
        var defaultReader = Console.In;
        try
        {
            var inputBuffer = new byte[1024];
            var inputStream = Console.OpenStandardInput(inputBuffer.Length);
            Console.SetIn(new StreamReader(inputStream, Console.InputEncoding, false, inputBuffer.Length));
        }
        catch
        {
            // May fail on some platforms
        }

        try
        {
            Console.Write(prompt + " ");
            return Console.ReadLine() ?? throw new EndOfStreamException();
        }
        finally
        {
            Console.SetIn(defaultReader);
        }
    }

    public void Write(object? output)
    {
        if (output == null) return;
        bool hasCustomToString = output.GetType().GetMethod(nameof(ToString))?.DeclaringType != typeof(object);
        Console.WriteLine(hasCustomToString
            ? output.ToString()
            : JsonSerializer.Serialize(output, options: new() {WriteIndented = true}));
    }

    public void WriteError(string output)
    {
        var previousColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine(output);
        Console.ForegroundColor = previousColor;
    }

    public void WriteError(Exception exception)
        => WriteError(exception.GetFullMessage());
}
