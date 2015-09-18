using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Entry point for commands operating on a REST API. Internally creates sub-<see cref="EndpointCommand"/>s.
    /// </summary>
    /// <typeparam name="TEndpoint">The specific type of <see cref="RestEntryPoint"/> to operate on.</typeparam>
    public abstract class EntryPointCommand<TEndpoint> : EndpointCommand
        where TEndpoint : RestEntryPoint
    {
        /// <summary>
        /// The REST endpoint this command operates on.
        /// </summary>
        protected readonly new TEndpoint Endpoint;

        /// <summary>
        /// Creates a new REST entry point command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        protected EntryPointCommand(TEndpoint endpoint) : base(endpoint)
        {
            Endpoint = endpoint;
        }

        public override async Task ExecuteAsync(IReadOnlyList<string> args)
        {
            var command = GetSubCommand(args[0]);
            await command.ExecuteAsync(args.Skip(1).ToList());
        }

        /// <summary>
        /// Creates a sub-<see cref="EndpointCommand"/> based on the given <paramref name="name"/>.
        /// </summary>
        protected abstract EndpointCommand GetSubCommand(string name);
    }
}