using System;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint that addresses a set of <typeparamref name="TElementEndpoint"/>s via IDs.
    /// </summary>
    /// <remarks>Use the more constrained <see cref="ICollectionEndpoint{TEntity}"/> when possible.</remarks>
    /// <typeparam name="TElementEndpoint">The type of <see cref="IEndpoint"/> to provide for individual elements.</typeparam>
    public class IndexerEndpoint<TElementEndpoint> : EndpointBase, IIndexerEndpoint<TElementEndpoint>
        where TElementEndpoint : class, IEndpoint
    {
        /// <summary>
        /// Creates a new element collection endpoint.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s.</param>
        public IndexerEndpoint(IEndpoint referrer, Uri relativeUri)
            : base(referrer, relativeUri)
        {
            SetupChildHandling();
        }

        /// <summary>
        /// Creates a new element collection endpoint.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Prefix <c>./</c> to append a trailing slash to the <paramref name="referrer"/> URI if missing.</param>
        public IndexerEndpoint(IEndpoint referrer, string relativeUri)
            : base(referrer, relativeUri)
        {
            SetupChildHandling();
        }

        private void SetupChildHandling()
        {
            SetDefaultLinkTemplate(rel: "child", href: "./{id}");
        }

        public Task ProbeAsync(CancellationToken cancellationToken = new CancellationToken())
            => HandleResponseAsync(HttpClient.OptionsAsync(Uri, cancellationToken));

        /// <summary>
        /// Builds a <typeparamref name="TElementEndpoint"/> for a specific child element of this collection. Does not perform any network traffic yet.
        /// </summary>
        /// <param name="relativeUri">The URI of the child endpoint relative to the this endpoint.</param>
        protected virtual TElementEndpoint BuildElementEndpoint(Uri relativeUri) => (TElementEndpoint)Activator.CreateInstance(typeof(TElementEndpoint), this, relativeUri);

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
