using TypedRest.CommandLine;
using TypedRest.Samples.Model;

namespace TypedRest.Samples.Client.CommandLine
{
    public class ResourceElementCommand : ElementCommand<Resource>
    {
        protected new readonly ResourceElement Endpoint;

        public ResourceElementCommand(ResourceElement endpoint) : base(endpoint)
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