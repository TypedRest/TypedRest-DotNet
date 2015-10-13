using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command operating on an <see cref="EntryEndpoint"/>. Internally creates sub-<see cref="IEndpointCommand"/>s.
    /// </summary>
    /// <typeparam name="TEndpoint">The specific type of <see cref="EntryEndpoint"/> to operate on.</typeparam>
    public abstract class EntryCommand<TEndpoint> : EndpointCommand<TEndpoint>
        where TEndpoint : EntryEndpoint
    {
        /// <summary>
        /// Creates a new REST entry point command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        protected EntryCommand(TEndpoint endpoint) : base(endpoint)
        {
        }

        public override async Task ExecuteAsync(IReadOnlyList<string> args,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var command = GetSubCommand(args[0]);
            await command.ExecuteAsync(args.Skip(1).ToList(), cancellationToken);
        }

        /// <summary>
        /// Creates a sub-<see cref="IEndpointCommand"/> based on the given <paramref name="name"/>.
        /// </summary>
        protected abstract IEndpointCommand GetSubCommand(string name);
    }
}