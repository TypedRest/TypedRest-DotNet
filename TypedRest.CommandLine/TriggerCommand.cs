using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command operating on an <see cref="IActionEndpoint"/>.
    /// </summary>
    public class TriggerCommand : EndpointCommand<IActionEndpoint>
    {
        /// <summary>
        /// Creates a new REST trigger command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public TriggerCommand(IActionEndpoint endpoint) : base(endpoint)
        {
        }

        protected override async Task ExecuteInnerAsync(IReadOnlyList<string> args,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await Endpoint.TriggerAsync(cancellationToken);
        }
    }
}