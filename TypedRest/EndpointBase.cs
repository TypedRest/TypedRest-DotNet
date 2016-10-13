using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TypedRest.UriTemplates;

namespace TypedRest
{
    /// <summary>
    /// Base class for building REST endpoints, i.e. remote HTTP resources.
    /// </summary>
    public abstract class EndpointBase : IEndpoint
    {
        public Uri Uri { get; }

        public HttpClient HttpClient { get; }

        public MediaTypeFormatter Serializer { get; }

        /// <summary>
        /// Creates a new REST endpoint with an absolute URI.
        /// </summary>
        /// <param name="uri">The HTTP URI of the remote element.</param>
        /// <param name="httpClient">The HTTP client used to communicate with the remote element.</param>
        /// <param name="serializer">Controls the serialization of entities sent to and received from the server.</param>
        protected EndpointBase(Uri uri, HttpClient httpClient, MediaTypeFormatter serializer)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));

            Uri = uri;
            HttpClient = httpClient;
            Serializer = serializer;
        }

        /// <summary>
        /// Creates a new REST endpoint with a relative URI.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s.</param>
        protected EndpointBase(IEndpoint referrer, Uri relativeUri) : this(
            uri: new Uri(referrer.Uri, relativeUri),
            httpClient: referrer.HttpClient,
            serializer: referrer.Serializer)
        {
        }

        /// <summary>
        /// Creates a new REST endpoint with a relative URI.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Prefix <c>./</c> to append a trailing slash to the <paramref name="referrer"/> URI if missing.</param>
        protected EndpointBase(IEndpoint referrer, string relativeUri) : this(
            uri: new Uri(relativeUri.StartsWith("./") ? referrer.Uri.EnsureTrailingSlash() : referrer.Uri, relativeUri),
            httpClient: referrer.HttpClient,
            serializer: referrer.Serializer)
        {
        }

        /// <summary>
        /// Registers one or more default links for a specific relation type.
        /// These links are used when no links with this relation type are provided by the server.
        /// </summary>
        /// <param name="rel">The relation type of the link to add.</param>
        /// <param name="hrefs">The hrefs of links relative to this endpoint's URI. Use <c>null</c> or an empty list to remove all previous entries for the relation type.</param>
        /// <remarks>This method is not thread-safe! Call this before performing any requests.</remarks>
        /// <seealso cref="IEndpoint.GetLinks"/>
        /// <seealso cref="IEndpoint.GetLinksWithTitles"/>
        /// <seealso cref="IEndpoint.Link"/>
        public void SetDefaultLink(string rel, params string[] hrefs)
        {
            if (hrefs == null || hrefs.Length == 0) _defaultLinks.Remove(rel);
            else _defaultLinks[rel] = new HashSet<Uri>(hrefs.Select(x => new Uri(Uri, x)));
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
        /// Handles the response of a REST request and wraps HTTP status codes in appropriate <see cref="Exception"/> types.
        /// </summary>
        /// <param name="responseTask">A response promise for a request that has started executing.</param>
        /// <returns>The resolved <paramref name="responseTask"/>.</returns>
        protected async Task<HttpResponseMessage> HandleResponseAsync(Task<HttpResponseMessage> responseTask)
        {
            var response = await responseTask.NoContext();

            await HandleLinksAsync(response).NoContext();
            HandleAllow(response);
            await HandleErrorsAsync(response).NoContext();

            return response;
        }

        /// <summary>
        /// Wraps HTTP status codes in appropriate <see cref="Exception"/> types.
        /// </summary>
        /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Unauthorized"/> or <see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException"><see cref="HttpStatusCode.Conflict"/>, <seealso cref="HttpStatusCode.PreconditionFailed"/> or <see cref="HttpStatusCode.RequestedRangeNotSatisfiable"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        protected virtual async Task HandleErrorsAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode) return;

            string message = $"{response.RequestMessage.RequestUri} responded with {(int)response.StatusCode} {response.ReasonPhrase}";

            string body = null;
            if (response.Content != null)
            {
                body = await response.Content.ReadAsStringAsync().NoContext();

                if (response.Content.Headers.ContentType?.MediaType == "application/json")
                {
                    try
                    {
                        var messageNode = JToken.Parse(body)["message"];
                        if (messageNode != null) message = messageNode.ToString();
                    }
                    catch (JsonException)
                    {
                    }
                }
            }

            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    throw new InvalidDataException(message, new HttpRequestException(body));
                case HttpStatusCode.Unauthorized:
                case HttpStatusCode.Forbidden:
                    throw new UnauthorizedAccessException(message, new HttpRequestException(body));
                case HttpStatusCode.NotFound:
                case HttpStatusCode.Gone:
                    throw new KeyNotFoundException(message, new HttpRequestException(body));
                case HttpStatusCode.Conflict:
                case HttpStatusCode.PreconditionFailed:
                case HttpStatusCode.RequestedRangeNotSatisfiable:
                    throw new InvalidOperationException(message, new HttpRequestException(body));
                //case HttpStatusCode.RequestedRangeNotSatisfiable:
                //    throw new VersionNotFoundException(message, new HttpRequestException(body));
                case HttpStatusCode.RequestTimeout:
                    throw new TimeoutException(message, new HttpRequestException(body));
                default:
                    throw new HttpRequestException(message, new HttpRequestException(body));
            }
        }

        /// <summary>
        /// Handles links embedded in an HTTP response.
        /// </summary>
        private async Task HandleLinksAsync(HttpResponseMessage response)
        {
            var links = new Dictionary<string, Dictionary<Uri, string>>();
            var linkTemplates = new Dictionary<string, UriTemplate>();

            HandleHeaderLinks(response.Headers, links, linkTemplates);
            await HandleBodyLinksAsync(response.Content, links, linkTemplates);

            _links = links;
            _linkTemplates = linkTemplates;
        }

        /// <summary>
        /// Handles links embedded in HTTP response headers.
        /// </summary>
        /// <param name="headers">The headers to check for links.</param>
        /// <param name="links">A dictionary to add found links to.</param>
        /// <param name="linkTemplates">A dictionary to add found link templates to.</param>
        protected virtual void HandleHeaderLinks(HttpResponseHeaders headers, IDictionary<string, Dictionary<Uri, string>> links, IDictionary<string, UriTemplate> linkTemplates)
        {
            foreach (var header in headers.GetLinkHeaders().Where(x => x.Rel != null))
            {
                if (header.Templated)
                    linkTemplates[header.Rel] = new UriTemplate(header.Href);
                else
                    links.GetOrAdd(header.Rel)[new Uri(Uri, header.Href)] = header.Title;
            }
        }

        /// <summary>
        /// Handles links embedded in JSON response bodies.
        /// </summary>
        /// <param name="content">The body to check for links.</param>
        /// <param name="links">A dictionary to add found links to.</param>
        /// <param name="linkTemplates">A dictionary to add found link templates to.</param>
        private async Task HandleBodyLinksAsync(HttpContent content, IDictionary<string, Dictionary<Uri, string>> links, IDictionary<string, UriTemplate> linkTemplates)
        {
            if (content?.Headers.ContentType?.MediaType == "application/json")
            {
                string json = await content.ReadAsStringAsync().NoContext();
                if (!string.IsNullOrEmpty(json))
                {
                    try
                    {
                        HandleBodyLinks(JToken.Parse(json), links, linkTemplates);
                    }
                    catch (JsonReaderException)
                    {
                        // Unparsable bodies are handled elsewhere
                    }
                }
            }
        }

        /// <summary>
        /// Handles links embedded in JSON response bodies.
        /// </summary>
        /// <param name="jsonBody">The body to check for links.</param>
        /// <param name="links">A dictionary to add found links to.</param>
        /// <param name="linkTemplates">A dictionary to add found link templates to.</param>
        protected virtual void HandleBodyLinks(JToken jsonBody, IDictionary<string, Dictionary<Uri, string>> links, IDictionary<string, UriTemplate> linkTemplates)
        {
            if (jsonBody.Type != JTokenType.Object) return;
            var linksNode = jsonBody["_links"] ?? jsonBody["links"];
            if (linksNode == null) return;

            foreach (var linkNode in linksNode.OfType<JProperty>())
            {
                string rel = linkNode.Name;
                var linksForRel = links.GetOrAdd(rel);

                switch (linkNode.Value.Type)
                {
                    case JTokenType.Array:
                        foreach (var subobj in linkNode.Value.OfType<JObject>())
                            ParseLinkObject(rel, subobj, linksForRel, linkTemplates);
                        break;

                    case JTokenType.Object:
                        ParseLinkObject(rel, (JObject)linkNode.Value, linksForRel, linkTemplates);
                        break;
                }
            }
        }

        /// <summary>
        /// Parses a JSON object for link information.
        /// </summary>
        /// <param name="rel">The relation type of the link.</param>
        /// <param name="obj">The JSON object to parse for link information.</param>
        /// <param name="linksForRel">A dictionary to add found links to. Maps hrefs to titles.</param>
        /// <param name="linkTemplates">A dictionary to add found link templates to. Maps rels to templated hrefs.</param>
        private void ParseLinkObject(string rel, JObject obj, IDictionary<Uri, string> linksForRel, IDictionary<string, UriTemplate> linkTemplates)
        {
            var href = obj["href"];
            if (href == null) return;

            var templated = obj["templated"];
            if (templated != null && templated.Type == JTokenType.Boolean && templated.Value<bool>())
                linkTemplates[rel] = new UriTemplate(href.ToString());
            else
            {
                var title = obj["title"];
                linksForRel[new Uri(Uri, href.ToString())] =
                    (title != null && title.Type == JTokenType.String) ? title.Value<string>() : null;
            }
        }

        // NOTE: Always replace entire dictionary rather than modifying it to ensure thread-safety.
        private Dictionary<string, Dictionary<Uri, string>> _links = new Dictionary<string, Dictionary<Uri, string>>();

        // NOTE: Only modify during initial setup
        private readonly Dictionary<string, HashSet<Uri>> _defaultLinks = new Dictionary<string, HashSet<Uri>>();

        public IEnumerable<Uri> GetLinks(string rel)
        {
            Dictionary<Uri, string> linksForRel;
            if (_links.TryGetValue(rel, out linksForRel))
                return linksForRel.Keys;

            HashSet<Uri> defaultLinksForRel;
            if (_defaultLinks.TryGetValue(rel, out defaultLinksForRel))
                return defaultLinksForRel;

            return new HashSet<Uri>();
        }

        public IDictionary<Uri, string> GetLinksWithTitles(string rel)
        {
            Dictionary<Uri, string> linksForRel;
            if (_links.TryGetValue(rel, out linksForRel))
                return linksForRel;

            HashSet<Uri> defaultLinksForRel;
            if (_defaultLinks.TryGetValue(rel, out defaultLinksForRel))
                return defaultLinksForRel.ToDictionary(x => x, x => (string)null);

            return new Dictionary<Uri, string>();
        }

        public Uri Link(string rel)
        {
            var uri = GetLinks(rel).FirstOrDefault();
            if (uri == null)
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

                uri = GetLinks(rel).FirstOrDefault();
                if (uri == null)
                    throw new KeyNotFoundException($"No link with rel={rel} provided by endpoint {Uri}.");
            }

            return uri;
        }

        // NOTE: Always replace entire dictionary rather than modifying it to ensure thread-safety.
        private Dictionary<string, UriTemplate> _linkTemplates = new Dictionary<string, UriTemplate>();

        // NOTE: Only modify during initial setup
        private readonly Dictionary<string, UriTemplate> _defaultLinkTemplates = new Dictionary<string, UriTemplate>();

        public UriTemplate LinkTemplate(string rel)
        {
            UriTemplate template;
            if (!_linkTemplates.TryGetValue(rel, out template) && !_defaultLinkTemplates.TryGetValue(rel, out template))
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
        {
            return new Uri(Uri, LinkTemplate(rel).Resolve(variables));
        }

        /// <summary>
        /// Handles allowed HTTP verbs reported by the server.
        /// </summary>
        private void HandleAllow(HttpResponseMessage response)
        {
            if (response.Content != null)
                _allowedVerbs = new HashSet<string>(response.Content.Headers.Allow);
        }

        // NOTE: Always replace entire set rather than modifying it to ensure thread-safety.
        private ISet<string> _allowedVerbs = new HashSet<string>();

        /// <summary>
        /// Shows whether the server has indicated that a specific HTTP verb is currently allowed.
        /// </summary>
        /// <param name="verb">The HTTP verb (e.g. GET, POST, ...) to check.</param>
        /// <remarks>Uses cached data from last response.</remarks>
        /// <returns>An indicator whether the verb is allowed. If no request has been sent yet or the server did not specify allowed verbs <c>null</c> is returned.</returns>
        protected bool? IsVerbAllowed(string verb)
        {
            if (_allowedVerbs.Count == 0) return null;
            return _allowedVerbs.Contains(verb);
        }

        public override string ToString()
        {
            return GetType().Name + ": " + Uri;
        }
    }
}