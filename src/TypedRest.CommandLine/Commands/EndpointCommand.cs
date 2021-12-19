using Newtonsoft.Json;
using TypedRest.CommandLine.IO;
using TypedRest.Endpoints;

namespace TypedRest.CommandLine.Commands
{
    /// <summary>
    /// Command operating on an <see cref="IEndpoint"/>.
    /// </summary>
    /// <typeparam name="TEndpoint">The specific type of <see cref="IEndpoint"/> to operate on.</typeparam>
    public abstract class EndpointCommand<TEndpoint> : IEndpointCommand
        where TEndpoint : IEndpoint
    {
        /// <summary>
        /// The endpoint this command operates on.
        /// </summary>
        protected readonly TEndpoint Endpoint;

        /// <summary>
        /// Creates a new endpoint command.
        /// </summary>
        /// <param name="endpoint">The endpoint this command operates on.</param>
        protected EndpointCommand(TEndpoint endpoint)
        {
            Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
        }

        /// <summary>
        /// The text input/output device used for user interaction.
        /// </summary>
        public IConsole Console { get; set; } = new JsonConsole();

        public virtual async Task ExecuteAsync(IReadOnlyList<string> args, CancellationToken cancellationToken = default)
        {
            var subCommand = (args.Count == 0) ? null : GetSubCommand(args[0]);
            if (subCommand == null) await ExecuteInnerAsync(args, cancellationToken);
            else await subCommand.ExecuteAsync(args.Skip(1).ToList(), cancellationToken);
        }

        /// <summary>
        /// Creates a sub-<see cref="IEndpointCommand"/> based on the given <paramref name="name"/>.
        /// </summary>
        /// <returns>The <see cref="IEndpointCommand"/> or <c>null</c> if the <paramref name="name"/> does not match.</returns>
        protected virtual IEndpointCommand? GetSubCommand(string name)
            => null;

        /// <summary>
        /// Parses command-line arguments and executes the resulting operation when no additional sub-<see cref="IEndpointCommand"/> is specified.
        /// </summary>
        /// <param name="args">the console arguments.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        protected virtual Task ExecuteInnerAsync(IReadOnlyList<string> args, CancellationToken cancellationToken = default)
            => throw new ArgumentException("Unknown command: " + args[0]);

        /// <summary>
        /// Reads an input object (usually in JSON format) either from the command-line or stdin.
        /// </summary>
        /// <param name="args">The command-line to check for input data.</param>
        /// <typeparam name="T">The type of object to read.</typeparam>
        protected virtual T Input<T>(IReadOnlyList<string> args)
            => ((args.Count == 0)
                   ? Console.Read<T>()
                   : JsonConvert.DeserializeObject<T>(args[0]))
            ?? throw new InvalidOperationException($"Expected {typeof(T)} but got null as input");
    }
}
