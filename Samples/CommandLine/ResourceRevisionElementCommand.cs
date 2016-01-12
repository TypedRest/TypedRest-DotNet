using TypedRest.CommandLine;
using TypedRest.Samples.Library.Endpoints;
using TypedRest.Samples.Library.Models;

namespace TypedRest.Samples.CommandLine
{
    public class ResourceRevisionElementCommand : ElementCommand<ResourceRevision>
    {
        protected new readonly ResourceRevisionElement Endpoint;

        public ResourceRevisionElementCommand(ResourceRevisionElement endpoint) : base(endpoint)
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