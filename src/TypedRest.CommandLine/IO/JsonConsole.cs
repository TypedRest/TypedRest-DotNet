using Newtonsoft.Json;

namespace TypedRest.CommandLine.IO;

/// <summary>
/// Reads from and writes to the default system console (stdin/stdout/stderr) using JSON serialization for objects.
/// </summary>
public class JsonConsole : IConsole
{
    public T? Read<T>()
        => JsonConvert.DeserializeObject<T>(Console.ReadLine() ?? throw new EndOfStreamException());

    public string Read(string prompt)
    {
        Console.Write(prompt + " ");
        return Console.ReadLine() ?? throw new EndOfStreamException();
    }

    public string ReadSecret(string prompt)
    {
        Console.Write(prompt + " ");
        var input = new StringBuilder();

        ConsoleKeyInfo key;
        while ((key = Console.ReadKey(intercept: true)).Key != ConsoleKey.Enter)
        {
            if (key.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                input.Length--;
                Console.Write("\b \b");
            }
            else if (!char.IsControl(key.KeyChar))
            {
                input.Append(key.KeyChar);
                Console.Write("*");
            }
        }

        Console.WriteLine();
        return input.ToString();
    }

    public void Write(object? output)
    {
        if (output == null) return;
        bool hasCustomToString = output.GetType().GetMethod(nameof(ToString))?.DeclaringType != typeof(object);
        Console.WriteLine(hasCustomToString
            ? output.ToString()
            : JsonConvert.SerializeObject(output, Formatting.Indented));
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
