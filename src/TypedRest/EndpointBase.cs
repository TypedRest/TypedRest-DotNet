using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using TypedRest.UriTemplates;

namespace TypedRest
{
    /// <summary>
    /// Base class for building endpoints, i.e. remote HTTP resources.
    /// </summary>
    public abstract class EndpointBase : IEndpoint
    {
        public Uri Uri { get; }
        public HttpClient HttpClient { get; }
        public MediaTypeFormatter Serializer { get; }
        public IErrorHandler ErrorHandler { get; }
        public ILinkHandler LinkHandler { get; }

        /// <summary>
        /// Creates a new endpoint with an absolute URI.
        /// </summary>
        /// <param name="uri">The HTTP URI of the remote element.</param>
        /// <param name="httpClient">The HTTP client used to communicate with the remote element.</param>
        /// <param name="serializer">Controls the serialization of entities sent to and received from the server.</param>
        /// <param name="errorHandler">Handles errors in HTTP responses.</param>
        /// <param name="linkHandler">Detects links in HTTP responses.</param>
        protected EndpointBase(Uri uri, HttpClient httpClient, MediaTypeFormatter serializer, IErrorHandler errorHandler, ILinkHandler linkHandler)
        {
            Uri = uri ?? throw new ArgumentNullException(nameof(uri));
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            ErrorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));
            LinkHandler = linkHandler ?? throw new ArgumentNullException(nameof(linkHandler));
        }

        /// <summary>
        /// Creates a new endpoint with a relative URI.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s.</param>
        protected EndpointBase(IEndpoint referrer, Uri relativeUri)
            : this(referrer.Uri.Join(relativeUri), referrer.HttpClient, referrer.Serializer, referrer.ErrorHandler, referrer.LinkHandler)
        {}

        /// <summary>
        /// Creates a new endpoint with a relative URI.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Prefix <c>./</c> to append a trailing slash to the <paramref name="referrer"/> URI if missing.</param>
        protected EndpointBase(IEndpoint referrer, string relativeUri)
            : this(referrer.Uri.Join(relativeUri), referrer.HttpClient, referrer.Serializer, referrer.ErrorHandler, referrer.LinkHandler)
        {}

        /// <summary>
        /// Registers one or more default links for a specific relation type.
        /// These links are used when no links with this relation type are provided by the server.
        /// </summary>
        /// <param name="rel">The relation type of the link to add.</param>
        /// <param name="hrefs">The hrefs of links relative to this endpoint's URI. Use <c>null</c> or an empty list to remove all previous entries for the relation type.</param>
        /// <remarks>This method is not thread-safe! Call this before performing any requests.</remarks>
        /// <seealso cref="IEndpoint.GetLinks"/>
        /// <seealso cref="IEndpoint.Link"/>
        public void SetDefaultLink(string rel, params string[] hrefs)
        {
            if (hrefs == null || hrefs.Length == 0) _defaultLinks.Remove(rel);
            else _defaultLinks[rel] = new HashSet<Link>(hrefs.Select(x => new Link(Uri.Join(x))));
        }

        /// <summary>
        /// Registers a default link template for a specific relation type.
        /// This template is used when no template with this relation type is provided by the server.
        /// </summary>
        /// <param name="rel">The relation type of the link template to add.</param>
        /// <param name="href">The href of the link template relative to this endpoint's URI. Use <c>null</c> to remove any previous entry for the relation type.</param>
        /// <remarks>This method is not thread-safe! Call this before performing any requests.</remarks>
        /// <seealso cref="IEndpoint.LinkTemplate(string)"/>
        /// <seealso cref="IEndpoint.LinkTemplate(string,object)"/>
        public void SetDefaultLinkTemplate(string rel, string href)
        {
            if (href == null) _defaultLinkTemplates.Remove(rel);
            else _defaultLinkTemplates[rel] = new UriTemplate(href);
        }

        /// <summary>
        /// Handles various cross-cutting concerns regarding a response message such as discovering links and handling errors.
        /// </summary>
        /// <param name="responseTask">A response promise for a request that has started executing.</param>
        /// <returns>The resolved <paramref name="responseTask"/>.</returns>
        protected virtual async Task<HttpResponseMessage> HandleResponseAsync(Task<HttpResponseMessage> responseTask)
        {
            var response = await responseTask.NoContext();

            (_links, _linkTemplates) = await LinkHandler.HandleAsync(response).NoContext();

            HandleCapabilities(response);

            if (!response.IsSuccessStatusCode)
                await ErrorHandler.HandleAsync(response).NoContext();

            return response;
        }

        // NOTE: Always replace entire dictionary rather than modifying it to ensure thread-safety.
        private LinkDictionary _links = new LinkDictionary();

        // NOTE: Only modify during initial setup
        private readonly IDictionary<string, ISet<Link>> _defaultLinks = new Dictionary<string, ISet<Link>>();

        public IEnumerable<Link> GetLinks(string rel)
        {
            if (_links.TryGetValue(rel, out var linksForRel))
                return linksForRel;

            if (_defaultLinks.TryGetValue(rel, out var defaultLinksForRel))
                return defaultLinksForRel;

            return new Link[0];
        }

        public Uri Link(string rel)
        {
            var link = GetLinks(rel).FirstOrDefault();
            if (link == null)
            {
                // Lazy lookup
                // NOTE: Synchronous execution so the method remains easy to use in constructors and properties
                Exception error = null;
                Task.Run(async () =>
                {
                    try
                    {
                        await HandleResponseAsync(HttpClient.HeadAsync(Uri)).NoContext();
                    }
                    catch (Exception ex)
                    {
                        error = ex;
                    }
                }).Wait();
                if (error != null)
                    throw new KeyNotFoundException($"No link with rel={rel} provided by endpoint {Uri}.", error);

                link = GetLinks(rel).FirstOrDefault();
                if (link == null)
                    throw new KeyNotFoundException($"No link with rel={rel} provided by endpoint {Uri}.");
            }

            return link.Href;
        }

        // NOTE: Always replace entire dictionary rather than modifying it to ensure thread-safety.
        private IDictionary<string, UriTemplate> _linkTemplates = new Dictionary<string, UriTemplate>();

        // NOTE: Only modify during initial setup
        private readonly IDictionary<string, UriTemplate> _defaultLinkTemplates = new Dictionary<string, UriTemplate>();

        public UriTemplate LinkTemplate(string rel)
        {
            if (!_linkTemplates.TryGetValue(rel, out var template) && !_defaultLinkTemplates.TryGetValue(rel, out template))
            {
                // Lazy lookup
                // NOTE: Synchronous execution so the method remains easy to use in constructors and properties
                Exception error = null;
                Task.Run(async () =>
                {
                    try
                    {
                        await HandleResponseAsync(HttpClient.HeadAsync(Uri)).NoContext();
                    }
                    catch (Exception ex)
                    {
                        error = ex;
                    }
                }).Wait();
                if (error != null)
                    throw new KeyNotFoundException($"No link template with rel={rel} provided by endpoint {Uri}.", error);

                if (_linkTemplates == null || !_linkTemplates.TryGetValue(rel, out template))
                    throw new KeyNotFoundException($"No link template with rel={rel} provided by endpoint {Uri}.");
            }

            return template;
        }

        public Uri LinkTemplate(string rel, object variables)
            => Uri.Join(LinkTemplate(rel).Resolve(variables));

        /// <summary>
        /// Handles allowed HTTP methods and other capabilities reported by the server.
        /// </summary>
        protected virtual void HandleCapabilities(HttpResponseMessage response)
        {
            if (response.Content != null)
                _allowedMethods = new HashSet<string>(response.Content.Headers.Allow);
        }

        // NOTE: Always replace entire set rather than modifying it to ensure thread-safety.
        private ISet<string> _allowedMethods = new HashSet<string>();

        /// <summary>
        /// Shows whether the server has indicated that a specific HTTP method is currently allowed.
        /// </summary>
        /// <param name="method">The HTTP methods (e.g. GET, POST, ...) to check.</param>
        /// <remarks>Uses cached data from last response.</remarks>
        /// <returns>An indicator whether the method is allowed. If no request has been sent yet or the server did not specify allowed methods <c>null</c> is returned.</returns>
        protected bool? IsMethodAllowed(HttpMethod method)
        {
            if (_allowedMethods.Count == 0) return null;
            return _allowedMethods.Contains(method.Method);
        }

        public override string ToString() => GetType().Name + ": " + Uri;
    }
}
