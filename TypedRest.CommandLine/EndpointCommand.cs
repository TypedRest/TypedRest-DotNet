using System.Collections.Generic;
using System.Threading.Tasks;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command operating on an <typeparamref name="TEndpoint"/>.
    /// </summary>
    /// <typeparam name="TEndpoint">The specific type of <see cref="IRestEndpoint"/> to operate on.</typeparam>
    public abstract class EndpointCommand<TEndpoint> : IEndpointCommand
        where TEndpoint : IRestEndpoint
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

        public abstract Task ExecuteAsync(IReadOnlyList<string> args);
    }
}