using System;
using TypedRest.Samples.Library.Models;

namespace TypedRest.Samples.Library.Endpoints
{
    /// <summary>
    /// REST endpoint that represents a <see cref="Resource"/>.
    /// </summary>
    public class ResourceElement : ElementEndpoint<Resource>
    {
        public ResourceElement(ResourceCollection parent, Uri relativeUri)
            : base(parent, relativeUri)
        {
        }

        /// <summary>
        /// Represents the <see cref="ResourceRevision"/>s.
        /// </summary>
        public ResourceRevisionCollection Revisions => new ResourceRevisionCollection(this);

        /// <summary>
        /// Exposes all <see cref="LogEvent"/>s that relate to this resource.
        /// </summary>
        public StreamEndpoint<LogEvent> Events => new StreamEndpoint<LogEvent>(this, Link("events"));
    }
}