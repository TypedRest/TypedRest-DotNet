using System;
using TypedRest.Samples.Library.Models;

namespace TypedRest.Samples.Library.Endpoints
{
    /// <summary>
    /// REST endpoint that represents a <see cref="Resource"/>.
    /// </summary>
    public class ResourceElement : ElementEndpoint<Resource>
    {
        /// <summary>
        /// Represents the <see cref="ResourceRevision"/>s.
        /// </summary>
        public ResourceRevisionCollection Revisions => new ResourceRevisionCollection(this);

        /// <summary>
        /// Exposes all <see cref="Event"/>s that relate to this resource.
        /// </summary>
        public StreamEndpoint<Event> Events => new StreamEndpoint<Event>(this, relativeUri: "events");

        public ResourceElement(ResourceCollection parent, Uri relativeUri) : base(parent, relativeUri)
        {
        }

        public ResourceElement(ResourceCollection parent, string relativeUri) : base(parent, relativeUri)
        {
        }
    }
}