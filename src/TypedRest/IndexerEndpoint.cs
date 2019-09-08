using System;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// Endpoint that addresses child <typeparamref name="TElementEndpoint"/>s by ID.
    /// </summary>
    /// <typeparam name="TElementEndpoint">The type of <see cref="IEndpoint"/> to provide for individual elements.</typeparam>
    public class IndexerEndpoint<TElementEndpoint> : EndpointBase, IIndexerEndpoint<TElementEndpoint>
        where TElementEndpoint : class, IEndpoint
    {
        private readonly Type _instanceType;

        /// <summary>
        /// Creates a new element collection endpoint.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s.</param>
        /// <param name="instanceType">The specific type to instantiate as element endpoints. Must derive from or implement <typeparamref name="TElementEndpoint"/>.</param>
        public IndexerEndpoint(IEndpoint referrer, Uri relativeUri, Type instanceType = null)
            : base(referrer, relativeUri)
        {
            _instanceType = CheckType(instanceType);
            SetupChildHandling();
        }

        /// <summary>
        /// Creates a new element collection endpoint.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s.</param>
        public IndexerEndpoint(IEndpoint referrer, Uri relativeUri)
            : this(referrer, relativeUri, typeof(TElementEndpoint))
        {}

        /// <summary>
        /// Creates a new element collection endpoint.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Prefix <c>./</c> to append a trailing slash to the <paramref name="referrer"/> URI if missing.</param>
        /// <param name="instanceType">The specific type to instantiate as element endpoints. Must derive from or implement <typeparamref name="TElementEndpoint"/>.</param>
        public IndexerEndpoint(IEndpoint referrer, string relativeUri, Type instanceType = null)
            : base(referrer, relativeUri)
        {
            _instanceType = CheckType(instanceType);
            SetupChildHandling();
        }

        /// <summary>
        /// Creates a new element collection endpoint.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Prefix <c>./</c> to append a trailing slash to the <paramref name="referrer"/> URI if missing.</param>
        public IndexerEndpoint(IEndpoint referrer, string relativeUri)
            : this(referrer, relativeUri, typeof(TElementEndpoint))
        {}

        private Type CheckType(Type instanceType)
        {
            if (instanceType == null) throw new ArgumentNullException(nameof(instanceType));
            if (!instanceType.IsClass || instanceType.IsAbstract) throw new ArgumentException("Must be a non-abstract class.", nameof(instanceType));
            if (!typeof(TElementEndpoint).IsAssignableFrom(instanceType)) throw new ArgumentException($"Must be assignable to {typeof(TElementEndpoint)}.", nameof(instanceType));

            return instanceType;
        }

        private void SetupChildHandling()
        {
            SetDefaultLinkTemplate(rel: "child", href: "./{id}");
        }

        public Task ProbeAsync(CancellationToken cancellationToken = default)
            => HandleResponseAsync(HttpClient.OptionsAsync(Uri, cancellationToken));

        /// <summary>
        /// Builds a <typeparamref name="TElementEndpoint"/> for a specific child element. Does not perform any network traffic yet.
        /// </summary>
        /// <param name="relativeUri">The URI of the child endpoint relative to the this endpoint.</param>
        protected virtual TElementEndpoint BuildElementEndpoint(Uri relativeUri)
            => (TElementEndpoint)Activator.CreateInstance(_instanceType, this, relativeUri);

        public virtual TElementEndpoint this[string id]
        {
            get
            {
                if (id == null) throw new ArgumentNullException(nameof(id));

                return BuildElementEndpoint(LinkTemplate("child", new {id}));
            }
        }
    }
}
