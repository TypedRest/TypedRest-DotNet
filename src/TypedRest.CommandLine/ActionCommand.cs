using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command operating on an <see cref="IActionEndpoint"/>.
    /// </summary>
    public class ActionCommand : EndpointCommand<IActionEndpoint>
    {
        /// <summary>
        /// Creates a new REST action command.
        /// </summary>
        /// <param name="endpoint">The endpoint this command operates on.</param>
        public ActionCommand(IActionEndpoint endpoint)
            : base(endpoint)
        {}

        protected override async Task ExecuteInnerAsync(IReadOnlyList<string> args,
                                                        CancellationToken cancellationToken = default)
            => await Endpoint.InvokeAsync(cancellationToken);
    }
}
