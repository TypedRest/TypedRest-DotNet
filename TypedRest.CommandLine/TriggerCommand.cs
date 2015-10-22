using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command operating on an <see cref="ITriggerEndpoint"/>.
    /// </summary>
    public class TriggerCommand : CommandBase<ITriggerEndpoint>
    {
        /// <summary>
        /// Creates a new REST trigger command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public TriggerCommand(ITriggerEndpoint endpoint) : base(endpoint)
        {
        }

        protected override async Task ExecuteInnerAsync(IReadOnlyList<string> args,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await Endpoint.TriggerAsync(cancellationToken);
        }
    }
}