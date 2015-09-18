using System.Collections.Generic;
using System.Threading.Tasks;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command operating on an <see cref="IRestEndpoint"/>.
    /// </summary>
    public abstract class EndpointCommand : IEndpointCommand
    {
        /// <summary>
        /// The REST endpoint this command operates on.
        /// </summary>
        protected readonly IRestEndpoint Endpoint;

        /// <summary>
        /// Creates a new REST endpoint command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        protected EndpointCommand(IRestEndpoint endpoint)
        {
            Endpoint = endpoint;
        }

        public abstract Task ExecuteAsync(IReadOnlyList<string> args);
    }
}