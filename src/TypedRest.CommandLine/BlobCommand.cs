using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command operating on an <see cref="IBlobEndpoint"/>.
    /// </summary>
    public class BlobCommand : EndpointCommand<IBlobEndpoint>
    {
        /// <summary>
        /// Creates a new REST blob command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public BlobCommand(IBlobEndpoint endpoint)
            : base(endpoint)
        {}

        protected override async Task ExecuteInnerAsync(IReadOnlyList<string> args,
                                                        CancellationToken cancellationToken = default)
        {
            switch (args[0])
            {
                case "download":
                    await Endpoint.DownloadAsync(args[1], cancellationToken);
                    break;

                case "upload":
                    await Endpoint.UploadFromAsync(args[1], args.Count > 2 ? args[2] : null, cancellationToken);
                    break;

                case "delete":
                    await Endpoint.DeleteAsync(cancellationToken);
                    break;

                default:
                    await base.ExecuteInnerAsync(args, cancellationToken);
                    break;
            }
        }
    }
}
