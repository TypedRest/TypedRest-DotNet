using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TypedRest
{
    /// <summary>
    /// Base class for building REST endpoints, i.e. remote HTTP resources.
    /// </summary>
    public abstract class EndpointBase : IEndpoint
    {
        public HttpClient HttpClient { get; private set; }

        public Uri Uri { get; private set; }

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
            : this(parent.HttpClient, new Uri(parent.Uri.EnsureTrailingSlash(), relativeUri))
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
            await HandleErrorsAsync(response);

            return response;
        }

        /// <summary>
        /// Wraps HTTP status codes in appropriate <see cref="Exception"/> types.
        /// </summary>
        /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Unauthorized"/> or <see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException"><see cref="HttpStatusCode.Conflict"/></exception>
        /// <exception cref="IndexOutOfRangeException"><see cref="HttpStatusCode.RequestedRangeNotSatisfiable"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        protected virtual async Task HandleErrorsAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode) return;

            string body = await response.Content.ReadAsStringAsync();
            string message = (response.Content.Headers.ContentType.MediaType == "application/json")
                ? JsonConvert.DeserializeAnonymousType(body, new {Message = ""}).Message
                : response.StatusCode + " " + response.ReasonPhrase;

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
                    throw new InvalidOperationException(message, new HttpRequestException(body));
                case HttpStatusCode.RequestedRangeNotSatisfiable:
                    throw new IndexOutOfRangeException(message, new HttpRequestException(body));
                default:
                    throw new HttpRequestException(message, new HttpRequestException(body));
            }
        }

        /// <summary>
        /// Handles HTTP Link headers.
        /// </summary>
        private void HandleLinks(HttpResponseMessage response)
        {
            foreach (string element in response.Headers.FirstOrDefault(x => x.Key == "Link").Value)
            {
                var properties = element.Split(new[] {"; "}, StringSplitOptions.None);
                string href = properties[0].Substring(1, properties[0].Length - 2);
                string rel = null, title = null;
                for (int i = 1; i < properties.Length; i++)
                {
                    var propertyParts = properties[i].Split('=');
                    if (propertyParts.Length == 2)
                    {
                        switch (propertyParts[0])
                        {
                            case "rel":
                                rel = propertyParts[1];
                                break;
                            case "title":
                                title = propertyParts[1];
                                break;
                        }
                    }
                }

                HandleLink(href, rel, title);
            }
        }

        /// <summary>
        /// Hook for handling links included in a response as an HTTP header.
        /// </summary>
        /// <param name="href">The URI the link points to.</param>
        /// <param name="rel">The relation type of the link; can be <see langword="null"/>.</param>
        /// <param name="title">A human-readable description of the link; can be <see langword="null"/>.</param>
        protected virtual void HandleLink(string href, string rel = null, string title = null)
        {
            if (rel == null)
            {
            }
            else if (rel == NotifyRel)
            {
                _notifyTargets.TryAdd(new Uri(Uri, href), true);
            }
            else if (rel.EndsWith("-template"))
            {
            }
            else
            {
                _links.AddOrUpdate(rel, new Uri(Uri, href), (_, x) => x);
            }
        }

        private readonly ConcurrentDictionary<string, Uri> _links = new ConcurrentDictionary<string, Uri>();

        public Uri Link(string rel)
        {
            // Try to lazy-load missing link data
            if (_links.Count == 0)
            {
                try
                {
                    HandleLinks(HttpClient.GetAsync(Uri).Result);
                }
                catch (Exception ex)
                {
                    throw new KeyNotFoundException($"No link with rel={rel} found in endpoint {Uri}.", ex);
                }
            }

            Uri uri;
            if (_links.TryGetValue(rel, out uri)) return uri;
            throw new KeyNotFoundException($"No link with rel={rel} found in endpoint {Uri}.");
        }

        /// <summary>
        /// The HTTP Link header relation type used by the server to set <see cref="IEndpoint.NotifyTargets"/>.
        /// </summary>
        public string NotifyRel { get; set; } = "notify";

        private readonly ConcurrentDictionary<Uri, bool> _notifyTargets = new ConcurrentDictionary<Uri, bool>();

        public IReadOnlyCollection<Uri> NotifyTargets => _notifyTargets.Keys.ToList();

        /// <summary>
        /// The <see cref="MediaTypeFormatter"/> used to serialize entities for transmission.
        /// </summary>
        protected virtual MediaTypeFormatter Serializer =>
            new JsonMediaTypeFormatter {SerializerSettings = {DefaultValueHandling = DefaultValueHandling.Ignore}};
    }
}