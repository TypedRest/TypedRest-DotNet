using System.Text.Json;
using TypedRest.CommandLine.Commands;
using TypedRest.Endpoints;

namespace TypedRest.CommandLine;

/// <summary>
/// Executes <see cref="IEndpointCommand"/>s based on command-line arguments.
/// </summary>
/// <typeparam name="TEndpoint">The type of entry endpoint to use for <see cref="CliEndpointProvider{T}"/>. Must have suitable constructors.</typeparam>
/// <typeparam name="TCommand">The type of entry command to use. Must have a constructor that takes a single <typeparamref name="TEndpoint"/>.</typeparam>
public class Executor<TEndpoint, TCommand>
    where TEndpoint : EntryEndpoint
    where TCommand : IEndpointCommand
{
    private readonly IEndpointProvider<TEndpoint> _endpointProvider;

    /// <summary>
    /// Creates an executor using the default <see cref="CliEndpointProvider{T}"/>.
    /// </summary>
    public Executor()
        : this(new CliEndpointProvider<TEndpoint>())
    {}

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
    /// <param name="args">the console arguments to parse.</param>
    /// <param name="cancellationToken">Used to cancel the request.</param>
    /// <returns>The exit code.</returns>
    public async Task<int> RunAsync(IReadOnlyList<string> args, CancellationToken cancellationToken = default)
    {
        TCommand BuildCommand() => NewCommand(_endpointProvider.Build());
        Task<int> ExecAsync() => ExecuteAsync(BuildCommand(), args, cancellationToken);

        try
        {
            int exitCode = await ExecAsync();
            switch (exitCode)
            {
                case 4:
                    _endpointProvider.ResetAuthentication();
                    return await ExecAsync();

                default:
                    return exitCode;
            }
        }
        #region Error handling
        catch (InvalidOperationException ex)
        {
            BuildCommand().Console.WriteError(ex);
            return 5;
        }
        #endregion
    }

    /// <summary>
    /// Instantiates a <typeparamref name="TCommand"/>.
    /// </summary>
    protected virtual TCommand NewCommand(TEndpoint endpoint)
        => (TCommand)Activator.CreateInstance(typeof(TCommand), endpoint)!;

    /// <summary>
    /// Executes a command and performs error handling.
    /// </summary>
    /// <param name="command">The command used to execute.</param>
    /// <param name="args">the console arguments to parse.</param>
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
            command.Console.WriteError("Missing arguments");
            return 1;
        }
        catch (ArgumentOutOfRangeException)
        {
            command.Console.WriteError("Missing arguments");
            return 1;
        }
        catch (ArgumentException ex)
        {
            command.Console.WriteError(ex);
            return 1;
        }
        catch (FormatException ex)
        {
            command.Console.WriteError(ex);
            return 1;
        }
        catch (InvalidDataException ex)
        {
            command.Console.WriteError(ex);
            return 2;
        }
        catch (UnauthorizedAccessException ex)
        {
            command.Console.WriteError(ex);
            return 3;
        }
        catch (AuthenticationException ex)
        {
            command.Console.WriteError(ex);
            return 4;
        }
        catch (KeyNotFoundException ex)
        {
            command.Console.WriteError(ex);
            return 5;
        }
        catch (InvalidOperationException ex)
        {
            command.Console.WriteError(ex);
            return 6;
        }
        catch (HttpRequestException ex)
        {
            command.Console.WriteError(ex);
            return 10;
        }
        catch (JsonException ex)
        {
            command.Console.WriteError(ex);
            return 11;
        }
        catch (IOException ex)
        {
            command.Console.WriteError(ex);
            return 12;
        }
        #endregion
    }
}
