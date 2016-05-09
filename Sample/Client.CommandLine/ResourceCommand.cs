using TypedRest.CommandLine;
using TypedRestSample.Model;

namespace TypedRestSample.Client.CommandLine
{
    public class ResourceCommand : ElementCommand<Resource>
    {
        protected new readonly ResourceEndpoint Endpoint;

        public ResourceCommand(ResourceEndpoint endpoint) : base(endpoint)
        {
            Endpoint = endpoint;
        }

        protected override IEndpointCommand GetSubCommand(string name)
        {
            switch (name.ToLowerInvariant())
            {
                case "revisions":
                    return new ResourceRevisionCollectionCommand(Endpoint.Revisions);
                case "events":
                    return new StreamCommand<LogEvent>(Endpoint.Events);
                default:
                    return base.GetSubCommand(name);
            }
        }
    }
}