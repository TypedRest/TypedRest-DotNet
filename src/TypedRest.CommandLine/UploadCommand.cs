using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command operating on an <see cref="IUploadEndpoint"/>.
    /// </summary>
    public class UploadCommand : EndpointCommand<IUploadEndpoint>
    {
        /// <summary>
        /// Creates a new REST upload command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public UploadCommand(IUploadEndpoint endpoint)
            : base(endpoint)
        {}

        protected override async Task ExecuteInnerAsync(IReadOnlyList<string> args,
                                                        CancellationToken cancellationToken = default(CancellationToken))
        {
            switch (args[0])
            {
                case "upload":
                    await Endpoint.UploadFromAsync(args[1], args.Count > 2 ? args[2] : null, cancellationToken);
                    break;

                default:
                    await base.ExecuteInnerAsync(args, cancellationToken);
                    break;
            }
        }
    }
}
