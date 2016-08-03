using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TypedRest.CommandLine;
using TypedRestSample.Model;

namespace TypedRestSample.Client.CommandLine
{
    public class ResourceRevisionCollectionCommand : CollectionCommandBase<ResourceRevision, ResourceRevisionCollectionEndpoint, ResourceRevisionEndpoint>
    {
        public ResourceRevisionCollectionCommand(ResourceRevisionCollectionEndpoint endpoint) : base(endpoint)
        {
        }

        protected override async Task ExecuteInnerAsync(IReadOnlyList<string> args,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (args.Count == 3 && args[0].ToLowerInvariant() == "create")
            {
                var newEntity = new ResourceRevision {Name = args[1]};
                var newEndpoint = await Endpoint.CreateAsync(newEntity, cancellationToken);
                using (var stream = File.OpenRead(args[2]))
                    await newEndpoint.Blob.UploadFromAsync(stream, cancellationToken: cancellationToken);
                Console.WriteLine(newEndpoint.Uri);
            }
            else await base.ExecuteInnerAsync(args, cancellationToken);
        }

        protected override IEndpointCommand GetSubCommand(string name)
        {
            switch (name.ToLowerInvariant())
            {
                case "working-latest":
                    return new ElementCommand<ResourceRevision>(Endpoint.Latest);
                default:
                    return base.GetSubCommand(name);
            }
        }

        protected override IEndpointCommand GetElementCommand(ResourceRevisionEndpoint element)
        {
            return new ResourceRevisionCommand(element);
        }
    }
}