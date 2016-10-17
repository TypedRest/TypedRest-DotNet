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
    /// Executes <see cref="IEndpointCommand"/>s based on command-line arguments.
    /// </summary>
    /// <typeparam name="TEndpoint">The type of entry endpoint to use for <see cref="EndpointProvider{T}"/>. Must have suitable constructors.</typeparam>
    /// <typeparam name="TCommand">The type of entry command to use. Must have a constructor that takes a single <typeparamref name="TEndpoint"/>.</typeparam>
    public class Executor<TEndpoint, TCommand>
        where TEndpoint : EntryEndpoint
        where TCommand : IEndpointCommand
    {
        private readonly IEndpointProvider<TEndpoint> _endpointProvider;

        /// <summary>
        /// Creates an executor using the default <see cref="EndpointProvider{T}"/>.
        /// </summary>
        public Executor()
        {
            _endpointProvider = new EndpointProvider<TEndpoint>();
        }

        /// <summary>
        /// Creates an executor using a custom <paramref name="endpointProvider"/>.
        /// </summary>
        public Executor(IEndpointProvider<TEndpoint> endpointProvider)
        {
            _endpointProvider = endpointProvider;
        }

        /// <summary>
        /// Creates a new endpoint and command and executes it using the specified command-line arguments.
        /// </summary>
        /// <param name="args">The command-line arguments to parse.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <returns>The exit code.</returns>
        public async Task<int> RunAsync(IReadOnlyList<string> args, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                int exitCode = await ExecuteAsync(NewCommand(_endpointProvider.Build()), args, cancellationToken);
                switch (exitCode)
                {
                    case 3:
                        _endpointProvider.ResetAuthentication();
                        return await ExecuteAsync(NewCommand(_endpointProvider.Build()), args, cancellationToken);

                    case 4:
                        _endpointProvider.ResetUri();
                        return await ExecuteAsync(NewCommand(_endpointProvider.Build()), args, cancellationToken);

                    default:
                        return exitCode;
                }
            }
                #region Error handling
            catch (InvalidOperationException ex)
            {
                PrintError(ex);
                return 5;
            }
            #endregion
        }

        /// <summary>
        /// Instantiates a <typeparamref name="TCommand"/>.
        /// </summary>
        protected virtual TCommand NewCommand(TEndpoint endpoint) => (TCommand)Activator.CreateInstance(typeof(TCommand), endpoint);

        /// <summary>
        /// Executes a command and performs error handling.
        /// </summary>
        /// <param name="command">The command used to execute.</param>
        /// <param name="args">The command-line arguments to parse.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <returns>The exit code.</returns>
        protected virtual async Task<int> ExecuteAsync(TCommand command, IReadOnlyList<string> args, CancellationToken cancellationToken)
        {
            try
            {
                await command.ExecuteAsync(args, cancellationToken);
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
                PrintError(ex);
                return 1;
            }
            catch (FormatException ex)
            {
                PrintError(ex);
                return 1;
            }
            catch (InvalidDataException ex)
            {
                PrintError(ex);
                return 2;
            }
            catch (UnauthorizedAccessException ex)
            {
                PrintError(ex);
                return 3;
            }
            catch (KeyNotFoundException ex)
            {
                PrintError(ex);
                return 4;
            }
            catch (InvalidOperationException ex)
            {
                PrintError(ex);
                return 5;
            }
            catch (HttpRequestException ex)
            {
                PrintError(ex);
                return 6;
            }
            catch (JsonException ex)
            {
                PrintError(ex);
                return 7;
            }
            catch (IOException ex)
            {
                PrintError(ex);
                return 6;
            }
            #endregion
        }

        /// <summary>
        /// Prints an <paramref name="exception"/> to the console.
        /// </summary>
        protected virtual void PrintError(Exception exception) => PrintError(exception.GetFullMessage());

        /// <summary>
        /// Prints an error <paramref name="message"/> to the console.
        /// </summary>
        protected virtual void PrintError(string message)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(message);
            Console.ForegroundColor = color;
        }
    }
}