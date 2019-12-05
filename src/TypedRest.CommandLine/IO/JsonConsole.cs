using System;
using Newtonsoft.Json;

#if NETFRAMEWORK
using System.IO;
#endif

namespace TypedRest.CommandLine.IO
{
    /// <summary>
    /// Reads from and writes to the default system console (stdin/stdout/stderr) using JSON serialization for objects.
    /// </summary>
    public class JsonConsole : IConsole
    {
        public T Read<T>()
            => JsonConvert.DeserializeObject<T>(Console.ReadLine());

        public string Read(string prompt)
        {
            Console.Write(prompt + " ");
            return Console.ReadLine();
        }

        public string ReadSecret(string prompt)
        {
#if NETFRAMEWORK
            // Increase maximum length for Console.ReadLine()
            var defaultReader = Console.In;
            var inputBuffer = new byte[1024];
            var inputStream = Console.OpenStandardInput(inputBuffer.Length);
            Console.SetIn(new StreamReader(inputStream, Console.InputEncoding, false, inputBuffer.Length));

            try
            {
#endif
                Console.Write(prompt + " ");
                return Console.ReadLine();
#if NETFRAMEWORK
            }
            finally
            {
                Console.SetIn(defaultReader);
            }
#endif
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
}
