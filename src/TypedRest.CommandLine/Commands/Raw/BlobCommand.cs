using TypedRest.Endpoints.Raw;

namespace TypedRest.CommandLine.Commands.Raw;

/// <summary>
/// Command operating on an <see cref="IBlobEndpoint"/>.
/// </summary>
/// <param name="endpoint">The endpoint this command operates on.</param>
public class BlobCommand(IBlobEndpoint endpoint) : EndpointCommand<IBlobEndpoint>(endpoint)
{
    protected override async Task ExecuteInnerAsync(IReadOnlyList<string> args, CancellationToken cancellationToken = default)
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
