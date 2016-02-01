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
        protected EndpointBase(IEndpoint parent, Uri relativeUri)
            : this(parent.HttpClient, new Uri(parent.Uri, relativeUri))
        {
        }

        /// <summary>
        /// Creates a new REST endpoint with a relative URI.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s.</param>
        protected EndpointBase(IEndpoint parent, string relativeUri)
            : this(parent, new Uri(relativeUri, UriKind.Relative))
        {
        }

        /// <summary>
        /// Handles the response of a REST request and wraps HTTP status codes in appropriate <see cref="Exception"/> types.
        /// </summary>
        /// <param name="responseTask">A response promise for a request that has started executing.</param>
        /// <returns>The resolved <paramref name="responseTask"/>.</returns>
        protected async Task<HttpResponseMessage> HandleResponseAsync(Task<HttpResponseMessage> responseTask)
        {
            var response = await responseTask;

            HandleLinks(response);
            HandleAllow(response);
            await HandleErrorsAsync(response);

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

            string message = response.StatusCode + " " + response.ReasonPhrase;

            string body = await response.Content.ReadAsStringAsync();

            if (response.Content.Headers.ContentType.MediaType == "application/json")
            {
                var messageNode = JToken.Parse(body)["message"];
                if (messageNode != null) message = messageNode.ToString();

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
        private void HandleLinks(HttpResponseMessage response)
        {
            var links = new Dictionary<string, ISet<Uri>>();
            var linkTemplates = new Dictionary<string, UriTemplate>();

            HandleHeaderLinks(response.Headers, links, linkTemplates);

            _links = links;
            _linkTemplates = linkTemplates;
        }

        /// <summary>
        /// Handles links embedded in HTTP response headers.
        /// </summary>
        /// <param name="headers">The headers to check for links.</param>
        /// <param name="links">A dictionary to add found links to.</param>
        /// <param name="linkTemplates">A dictionary to add found link templates to.</param>
        protected virtual void HandleHeaderLinks(HttpResponseHeaders headers, IDictionary<string, ISet<Uri>> links, IDictionary<string, UriTemplate> linkTemplates)
        {
            foreach (string element in headers.Where(x => x.Key == "Link").SelectMany(x => x.Value))
            {
                var parameters = element.Split(new[] {"; "}, StringSplitOptions.None);
                string href = parameters[0].Substring(1, parameters[0].Length - 2);

                var relParameter = parameters.Select(x => x.Split('=')).FirstOrDefault(x => x[0] == "rel");
                if (relParameter != null)
                {
                    var templatedParameter = parameters.Select(x => x.Split('=')).FirstOrDefault(x => x[0] == "templated");
                    if (templatedParameter != null && templatedParameter[1] == "true")
                    {
                        string rel = relParameter[1];
                        linkTemplates[rel] = new UriTemplate(href);
                    }
                    else
                    {
                        string rel = relParameter[1];
                        ISet<Uri> linkSet;
                        if (!links.TryGetValue(rel, out linkSet))
                            links.Add(rel, linkSet = new HashSet<Uri>());
                        linkSet.Add(new Uri(Uri, href));
                    }
                }
            }
        }

        // NOTE: Always replace entire dictionary rather than modifying it to ensure thread-safety.
        private IDictionary<string, ISet<Uri>> _links = new Dictionary<string, ISet<Uri>>();

        public IEnumerable<Uri> GetLinks(string rel)
        {
            ISet<Uri> uris;
            return _links.TryGetValue(rel, out uris) ? uris : Enumerable.Empty<Uri>();
        }

        public Uri Link(string rel)
        {
            var uri = GetLinks(rel).FirstOrDefault();
            if (uri == null)
            {
                // Lazy lookup
                try
                {
                    // NOTE: Synchronous execution so the method remains easy to use in constructors and properties
                    Task.Run(() => HandleResponseAsync(HttpClient.HeadAsync(Uri))).Wait();
                }
                catch (Exception ex)
                {
                    throw new KeyNotFoundException($"No link with rel={rel} provided by endpoint {Uri}.", ex);
                }

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
                try
                {
                    // NOTE: Synchronous execution so the method remains easy to use in constructors and properties
                    Task.Run(() => HandleResponseAsync(HttpClient.HeadAsync(Uri))).Wait();
                }
                catch (Exception)
                {
                    // HTTP HEAD server-side implementation is optional
                }

                _linkTemplates.TryGetValue(rel, out template);
            }

            return template;
        }

        /// <summary>
        /// Handles allowed HTTP verbs reported by the server.
        /// </summary>
        private void HandleAllow(HttpResponseMessage response)
        {
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
    }
}