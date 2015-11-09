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
        /// Exposes all <see cref="LogEvent"/>s that relate to this resource.
        /// </summary>
        public StreamEndpoint<LogEvent> Events => new StreamEndpoint<LogEvent>(this, relativeUri: "events");

        public ResourceElement(IEndpoint parent, Uri relativeUri) : base(parent, relativeUri)
        {
        }

        public ResourceElement(IEndpoint parent, string relativeUri) : base(parent, relativeUri)
        {
        }
    }
}