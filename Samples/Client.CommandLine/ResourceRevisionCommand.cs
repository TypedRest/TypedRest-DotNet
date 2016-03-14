using TypedRest.CommandLine;
using TypedRest.Samples.Model;

namespace TypedRest.Samples.Client.CommandLine
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
                    return new TriggerCommand(Endpoint.Promote);
                default:
                    return base.GetSubCommand(name);
            }
        }
    }
}