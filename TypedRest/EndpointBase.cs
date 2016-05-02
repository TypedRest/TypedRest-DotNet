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
        public HttpClient HttpClient { get; }

        public Uri Uri { get; }

        /// <summary>
        /// Creates a new REST endpoint with an absolute URI.
        /// </summary>
        /// <param name="httpClient">The HTTP client used to communicate with the remote element.</param>
        /// <param name="uri">The HTTP URI of the remote element.</param>
        protected EndpointBase(HttpClient httpClient, Uri uri)
        {
            if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            HttpClient = httpClient;
            Uri = uri;
        }

        /// <summary>
        /// Creates a new REST endpoint with a relative URI.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s.</param>
        /// <param name="ensureTrailingSlashOnParentUri">If true, ensures a trailing slash on the parent uri.</param>
        protected EndpointBase(IEndpoint parent, Uri relativeUri, bool ensureTrailingSlashOnParentUri = false)
            : this(parent.HttpClient, new Uri(ensureTrailingSlashOnParentUri ? parent.Uri.EnsureTrailingSlash() : parent.Uri, relativeUri))
        {
        }

        /// <summary>
        /// Creates a new REST endpoint with a relative URI.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s.</param>
        /// <param name="ensureTrailingSlashOnParentUri">If true, ensures a trailing slash on the parent uri.</param>
        protected EndpointBase(IEndpoint parent, string relativeUri, bool ensureTrailingSlashOnParentUri = false)
            : this(parent, new Uri(relativeUri, UriKind.Relative), ensureTrailingSlashOnParentUri)
        {
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
                default:
                    throw new HttpRequestException(message, new HttpRequestException(body));
            }
        }

        /// <summary>
        /// Handles links embedded in an HTTP response.
        /// </summary>
        private async Task HandleLinksAsync(HttpResponseMessage response)
        {
            var links = new Dictionary<string, IDictionary<Uri, string>>();
            var linkTemplates = new Dictionary<string, UriTemplate>();

            HandleHeaderLinks(response.Headers, links, linkTemplates);

            if (response.Content?.Headers.ContentType?.MediaType == "application/json")
            {
                try
                {
                    HandleBodyLinks(JToken.Parse(await response.Content.ReadAsStringAsync().NoContext()), links, linkTemplates);
                }
                catch (JsonReaderException)
                {
                    // Unparsable bodies are handled elsewhere
                }
            }

            _links = links;
            _linkTemplates = linkTemplates;
        }

        /// <summary>
        /// Handles links embedded in HTTP response headers.
        /// </summary>
        /// <param name="headers">The headers to check for links.</param>
        /// <param name="links">A dictionary to add found links to.</param>
        /// <param name="linkTemplates">A dictionary to add found link templates to.</param>
        protected virtual void HandleHeaderLinks(HttpResponseHeaders headers, IDictionary<string, IDictionary<Uri, string>> links, IDictionary<string, UriTemplate> linkTemplates)
        {
            foreach (string element in headers.Where(x => x.Key == "Link").SelectMany(x => x.Value)
                .SelectMany(x => x.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)).Select(x => x.Trim()))
            {
                var components = element.Split(new[] {"; "}, StringSplitOptions.None);
                string href = components[0].Substring(1, components[0].Length - 2);

                var parameters = components.Skip(1).Select(x => x.Split('=')).ToLookup(x => x[0], x => x[1]);

                string rel = parameters["rel"].FirstOrDefault();
                if (rel != null)
                {
                    string templated = parameters["templated"].FirstOrDefault();
                    if (templated == "true")
                        linkTemplates[rel] = new UriTemplate(href);
                    else
                    {
                        IDictionary<Uri, string> linksForRel;
                        if (!links.TryGetValue(rel, out linksForRel))
                            links.Add(rel, linksForRel = new Dictionary<Uri, string>());

                        string title = parameters["title"].FirstOrDefault();

                        linksForRel.Add(new Uri(Uri, href), title);
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
        protected virtual void HandleBodyLinks(JToken jsonBody, IDictionary<string, IDictionary<Uri, string>> links, IDictionary<string, UriTemplate> linkTemplates)
        {
            if (jsonBody.Type != JTokenType.Object) return;
            var linksNode = jsonBody["_links"] ?? jsonBody["links"];
            if (linksNode == null) return;

            foreach (var linkNode in linksNode.OfType<JProperty>())
            {
                string rel = linkNode.Name;

                IDictionary<Uri, string> linksForRel;
                if (!links.TryGetValue(rel, out linksForRel))
                    links.Add(rel, linksForRel = new Dictionary<Uri, string>());

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
                linkTemplates.Add(rel, new UriTemplate(href.ToString()));
            else
            {
                var title = obj["title"];
                linksForRel.Add(
                    new Uri(Uri, href.ToString()),
                    (title != null && title.Type == JTokenType.String) ? title.Value<string>() : null);
            }
        }

        // NOTE: Always replace entire dictionary rather than modifying it to ensure thread-safety.
        private IDictionary<string, IDictionary<Uri, string>> _links = new Dictionary<string, IDictionary<Uri, string>>();

        public IEnumerable<Uri> GetLinks(string rel)
        {
            return GetLinksWithTitles(rel).Keys;
        }

        public IDictionary<Uri, string> GetLinksWithTitles(string rel)
        {
            IDictionary<Uri, string> linksForRel;
            return _links.TryGetValue(rel, out linksForRel) ? linksForRel : new Dictionary<Uri, string>();
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
        private IDictionary<string, UriTemplate> _linkTemplates = new Dictionary<string, UriTemplate>();

        public UriTemplate LinkTemplate(string rel)
        {
            UriTemplate template;
            if (!_linkTemplates.TryGetValue(rel, out template))
            {
                // Lazy lookup
                // NOTE: Synchronous execution so the method remains easy to use in constructors and properties
                Task.Run(async () =>
                {
                    try
                    {
                        await HandleResponseAsync(HttpClient.HeadAsync(Uri)).NoContext();
                    }
                    catch (Exception)
                    {
                        // HTTP HEAD server-side implementation is optional
                    }
                }).Wait();

                _linkTemplates.TryGetValue(rel, out template);
            }

            return template;
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

        /// <summary>
        /// The <see cref="MediaTypeFormatter"/> used to serialize entities for transmission.
        /// </summary>
        protected virtual MediaTypeFormatter Serializer =>
            new JsonMediaTypeFormatter {SerializerSettings = {DefaultValueHandling = DefaultValueHandling.Ignore}};

        public override string ToString()
        {
            return GetType().Name + ": " + Uri;
        }
    }
}