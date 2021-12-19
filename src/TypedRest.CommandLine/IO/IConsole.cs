namespace TypedRest.CommandLine.IO;

/// <summary>
/// Represents a text input/output device (e.g. a terminal/console) and a serialization strategy for objects.
/// </summary>
public interface IConsole
{
    /// <summary>
    /// Reads an input object (usually in JSON format).
    /// </summary>
    /// <typeparam name="T">The type of object to read.</typeparam>
    T? Read<T>();

    /// <summary>
    /// Reads a text input.
    /// </summary>
    /// <param name="prompt">The prompt to ask the user.</param>
    string Read(string prompt);

    /// <summary>
    /// Reads a secret text input.
    /// </summary>
    /// <param name="prompt">The prompt to ask the user.</param>
    string ReadSecret(string prompt);

    /// <summary>
    /// Writes an output object (usually in JSON format).
    /// </summary>
    void Write(object? output);

    /// <summary>
    /// Writes an error message.
    /// </summary>
    void WriteError(string output);

    /// <summary>
    /// Writes an exception as an error message.
    /// </summary>
    void WriteError(Exception exception);
}