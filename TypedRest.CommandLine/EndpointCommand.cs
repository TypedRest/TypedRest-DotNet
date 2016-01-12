using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command operating on an <see cref="IEndpoint"/>.
    /// </summary>
    /// <typeparam name="TEndpoint">The specific type of <see cref="IEndpoint"/> to operate on.</typeparam>
    public abstract class EndpointCommand<TEndpoint> : IEndpointCommand
        where TEndpoint : IEndpoint
    {
        /// <summary>
        /// The REST endpoint this command operates on.
        /// </summary>
        protected readonly TEndpoint Endpoint;

        /// <summary>
        /// Creates a new REST endpoint command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        protected EndpointCommand(TEndpoint endpoint)
        {
            Endpoint = endpoint;
        }

        public virtual async Task ExecuteAsync(IReadOnlyList<string> args,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var subCommand = (args.Count == 0) ? null : GetSubCommand(args[0]);
            if (subCommand == null) await ExecuteInnerAsync(args, cancellationToken);
            else await subCommand.ExecuteAsync(args.Skip(1).ToList(), cancellationToken);
        }

        /// <summary>
        /// Creates a sub-<see cref="IEndpointCommand"/> based on the given <paramref name="name"/>.
        /// </summary>
        /// <returns>The <see cref="IEndpointCommand"/> or <c>null</c> if the <paramref name="name"/> does not match.</returns>
        protected virtual IEndpointCommand GetSubCommand(string name)
        {
            return null;
        }

        /// <summary>
        /// Parses command-line arguments and executes the resulting operation when no additional sub-<see cref="IEndpointCommand"/> is specified.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        protected virtual Task ExecuteInnerAsync(IReadOnlyList<string> args,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new ArgumentException("Unknown command: " + args[0]);
        }
    }
}