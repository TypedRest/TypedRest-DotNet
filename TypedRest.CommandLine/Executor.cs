using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Entry point for <see cref="IEndpointCommand"/> command-line execution.
    /// </summary>
    public static class Executor
    {
        /// <summary>
        /// Executes commands based in command-line arguments. Performs error handling and reporting.
        /// </summary>
        /// <param name="entryCommand">The top-level command used to locate sub-commands.</param>
        /// <param name="args">The command-line arguments to parse.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <returns>The exit code.</returns>
        public static async Task<int> RunAsync(IEndpointCommand entryCommand, IReadOnlyList<string> args,
            CancellationToken cancellationToken)
        {
            try
            {
                await entryCommand.ExecuteAsync(args, cancellationToken);
                return 0;
            }
                #region Error handling
            catch (OperationCanceledException)
            {
                return 99;
            }
            catch (IndexOutOfRangeException)
            {
                PrintError("Missing arguments");
                return 1;
            }
            catch (ArgumentOutOfRangeException)
            {
                PrintError("Missing arguments");
                return 1;
            }
            catch (ArgumentException ex)
            {
                PrintError(ex.GetFullMessage());
                return 1;
            }
            catch (FormatException ex)
            {
                PrintError(ex.GetFullMessage());
                return 1;
            }
            catch (InvalidDataException ex)
            {
                PrintError(ex.GetFullMessage());
                return 2;
            }
            catch (UnauthorizedAccessException ex)
            {
                PrintError(ex.Message);
                return 3;
            }
            catch (KeyNotFoundException ex)
            {
                PrintError(ex.Message);
                return 4;
            }
            catch (InvalidOperationException ex)
            {
                PrintError(ex.Message);
                return 5;
            }
            catch (HttpRequestException ex)
            {
                PrintError(ex.GetFullMessage());
                return 6;
            }
            catch (JsonSerializationException ex)
            {
                PrintError(ex.GetFullMessage());
                return 7;
            }
            #endregion
        }

        private static void PrintError(string message)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(message);
            Console.ForegroundColor = color;
        }

        /// <summary>
        /// Executes commands based in command-line arguments. Performs error handling and reporting and cancels on Ctrl+C.
        /// </summary>
        /// <param name="entryCommand">The top-level command used to locate sub-commands.</param>
        /// <param name="args">The command-line arguments to parse.</param>
        /// <returns>The exit code.</returns>
        public static Task<int> RunAsync(IEndpointCommand entryCommand, IReadOnlyList<string> args)
        {
            return RunAsync(entryCommand, args, GetCancellationToken());
        }

        private static CancellationToken GetCancellationToken()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) =>
            {
                cancellationTokenSource.Cancel();

                // Allow the application to finish cleanup rather than terminating immediately
                e.Cancel = true;
            };
            return cancellationTokenSource.Token;
        }
    }
}