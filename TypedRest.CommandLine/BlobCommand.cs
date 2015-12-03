using System.Collections.Generic;
using System.IO;
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
        public BlobCommand(IBlobEndpoint endpoint) : base(endpoint)
        {
        }

        protected override async Task ExecuteInnerAsync(IReadOnlyList<string> args,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            switch (args[0])
            {
                case "download":
                    using (var stream = File.Create(args[1]))
                        await Endpoint.DownloadToAsync(stream);
                    break;

                case "upload":
                    using (var stream = File.OpenRead(args[1]))
                        await Endpoint.UploadFromAsync(stream);
                    break;

                default:
                    await base.ExecuteInnerAsync(args, cancellationToken);
                    break;
            }
        }
    }
}