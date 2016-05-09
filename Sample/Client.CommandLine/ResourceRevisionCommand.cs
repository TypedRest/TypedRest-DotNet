using TypedRest.CommandLine;
using TypedRestSample.Model;

namespace TypedRestSample.Client.CommandLine
{
    public class ResourceRevisionCommand : ElementCommand<ResourceRevision>
    {
        protected new readonly ResourceRevisionEndpoint Endpoint;

        public ResourceRevisionCommand(ResourceRevisionEndpoint endpoint) : base(endpoint)
        {
            Endpoint = endpoint;
        }

        protected override IEndpointCommand GetSubCommand(string name)
        {
            switch (name.ToLowerInvariant())
            {
                case "blob":
                    return new BlobCommand(Endpoint.Blob);
                case "promote":
                    return new ActionCommand(Endpoint.Promote);
                default:
                    return base.GetSubCommand(name);
            }
        }
    }
}