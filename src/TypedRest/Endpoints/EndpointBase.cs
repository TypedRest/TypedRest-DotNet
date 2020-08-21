using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Resta.UriTemplates;
using TypedRest.Errors;
using TypedRest.Http;
using TypedRest.Links;

namespace TypedRest.Endpoints
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
        public ILinkExtractor LinkExtractor { get; }

        /// <summary>
        /// Creates a new endpoint with an absolute URI.
        /// </summary>
        /// <param name="uri">The HTTP URI of the remote element.</param>
        /// <param name="httpClient">The HTTP client used to communicate with the remote element.</param>
        /// <param name="serializer">Controls the serialization of entities sent to and received from the server.</param>
        /// <param name="errorHandler">Handles errors in HTTP responses.</param>
        /// <param name="linkExtractor">Detects links in HTTP responses.</param>
        protected EndpointBase(Uri uri, HttpClient httpClient, MediaTypeFormatter serializer, IErrorHandler errorHandler, ILinkExtractor linkExtractor)
        {
            Uri = uri ?? throw new ArgumentNullException(nameof(uri));
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            ErrorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));
            LinkExtractor = linkExtractor ?? throw new ArgumentNullException(nameof(linkExtractor));
        }

        /// <summary>
        /// Creates a new endpoint with a relative URI.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s.</param>
        protected EndpointBase(IEndpoint referrer, Uri relativeUri)
            : this(referrer.Uri.Join(relativeUri), referrer.HttpClient, referrer.Serializer, referrer.ErrorHandler, referrer.LinkExtractor)
        {}

        /// <summary>
        /// Creates a new endpoint with a relative URI.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Prefix <c>./</c> to append a trailing slash to the <paramref name="referrer"/> URI if missing.</param>
        protected EndpointBase(IEndpoint referrer, string relativeUri)
            : this(referrer.Uri.Join(relativeUri), referrer.HttpClient, referrer.Serializer, referrer.ErrorHandler, referrer.LinkExtractor)
        {}

        /// <summary>
        /// Registers one or more default links for a specific relation type.
        /// These links are used when no links with this relation type are provided by the server.
        /// </summary>
        /// <param name="rel">The relation type of the link to add.</param>
        /// <param name="href">The href of the link relative to this endpoint's URI. Use <c>null</c> to remove any previous entries for the relation type.</param>
        /// <remarks>This method is not thread-safe! Call this before performing any requests.</remarks>
        /// <seealso cref="IEndpoint.GetLinks"/>
        /// <seealso cref="IEndpoint.Link"/>
        public void SetDefaultLink(string rel, string? href)
        {
            if (string.IsNullOrEmpty(href)) _defaultLinks.Remove(rel);
            else _defaultLinks[rel] = Uri.Join(href);
        }

        /// <summary>
        /// Registers a default link template for a specific relation type.
        /// This template is used when no template with this relation type is provided by the server.
        /// </summary>
        /// <param name="rel">The relation type of the link template to add.</param>
        /// <param name="href">The href of the link template relative to this endpoint's URI. Use <c>null</c> to remove any previous entry for the relation type.</param>
        /// <remarks>This method is not thread-safe! Call this before performing any requests.</remarks>
        /// <seealso cref="IEndpoint.LinkTemplate(string,object)"/>
        public void SetDefaultLinkTemplate(string rel, string? href)
        {
            if (string.IsNullOrEmpty(href)) _defaultLinkTemplates.Remove(rel);
            else _defaultLinkTemplates[rel] = new UriTemplate(href);
        }

        /// <summary>
        /// Handles various cross-cutting concerns regarding a response message such as discovering links and handling errors.
        /// </summary>
        /// <param name="request">A callback that performs the actual HTTP request.</param>
        /// <param name="caller">The name of the method calling this method.</param>
        /// <returns>The resolved <paramref name="request"/>.</returns>
        protected virtual Task<HttpResponseMessage> HandleAsync(Func<Task<HttpResponseMessage>> request, [CallerMemberName] string caller = "unknown")
            => TracedAsync(async activity =>
            {
                var response = await request().NoContext();
                activity.AddTag("http.method", response.RequestMessage.Method.Method)
                        .AddTag("http.status_code", ((int)response.StatusCode).ToString());

                _links = await LinkExtractor.GetLinksAsync(response).NoContext();

                HandleCapabilities(response);

                if (!response.IsSuccessStatusCode)
                    await ErrorHandler.HandleAsync(response).NoContext();

                return response;
            });

        /// <summary>
        /// Runs an operation in the context of a tracing span.
        /// </summary>
        /// <param name="operation">Callback to run the operation. Passes in an <see cref="Activity"/> to record additional tracing information.</param>
        /// <param name="caller">The name of the method calling this method.</param>
        protected async Task TracedAsync(Func<Activity, Task> operation, [CallerMemberName] string caller = "unknown")
        {
            var activity = StartActivity(caller);

            try
            {
                await operation(activity).NoContext();
            }
            catch (Exception ex)
            {
                activity.AddException(ex);
                throw;
            }
            finally
            {
                activity.Stop();
            }
        }

        /// <summary>
        /// Runs an operation in the context of a tracing span.
        /// </summary>
        /// <param name="operation">Callback to run the operation. Passes in an <see cref="Activity"/> to record additional tracing information.</param>
        /// <param name="caller">The name of the method calling this method.</param>
        protected async Task<T> TracedAsync<T>(Func<Activity, Task<T>> operation, [CallerMemberName] string caller = "unknown")
        {
            var activity = StartActivity(caller);

            try
            {
                return await operation(activity).NoContext();
            }
            catch (Exception ex)
            {
                activity.AddException(ex);
                throw;
            }
            finally
            {
                activity.Stop();
            }
        }

        private Activity StartActivity(string caller)
        {
            if (caller.EndsWith("Async")) caller = caller.Substring(0, caller.Length - "Async".Length);

            return new Activity(GetType().Name + "::" + caller)
                  .AddTag("component", "TypedRest")
                  .AddTag("span.kind", "client")
                  .AddTag("http.url", Uri.ToString())
                  .Start();
        }

        // NOTE: Always replace entire dictionary rather than modifying it to ensure thread-safety.
        private IReadOnlyList<Link> _links = new Link[0];

        // NOTE: Only modify during initial setup
        private readonly IDictionary<string, Uri> _defaultLinks = new Dictionary<string, Uri>();

        // NOTE: Only modify during initial setup
        private readonly IDictionary<string, UriTemplate> _defaultLinkTemplates = new Dictionary<string, UriTemplate>();

        public IReadOnlyList<(Uri uri, string? title)> GetLinks(string rel)
        {
            var links = _links.Where(x => !x.Templated && x.Rel == rel)
                              .Select(x => (Uri.Join(x.Href), x.Title))
                              .ToList();

            if (links.Count == 0)
            {
                if (_defaultLinks.TryGetValue(rel, out var defaultLink))
                    links.Add((defaultLink, null));
            }

            return links;
        }

        public Uri Link(string rel)
        {
            var links = GetLinks(rel);
            if (links.Count == 0)
            {
                // Lazy lookup
                // NOTE: Synchronous execution so the method remains easy to use in constructors and properties
                Exception? error = null;
                Task.Run(async () =>
                {
                    try
                    {
                        await HandleAsync(() => HttpClient.HeadAsync(Uri)).NoContext();
                    }
                    catch (Exception ex)
                    {
                        error = ex;
                    }
                }).Wait();
                if (error != null)
                    throw new KeyNotFoundException($"No link with rel={rel} provided by endpoint {Uri}.", error);

                links = GetLinks(rel);
                if (links.Count == 0)
                    throw new KeyNotFoundException($"No link with rel={rel} provided by endpoint {Uri}.");
            }

            return links[0].uri;
        }

        /// <summary>
        /// Retrieves a link template with a specific relation type.
        /// </summary>
        /// <param name="rel">The relation type of the link template to look for.</param>
        /// <returns>The unresolved link template.</returns>
        /// <exception cref="KeyNotFoundException">No link template with the specified <paramref name="rel"/> could be found.</exception>
        /// <remarks>Uses cached data from last response if possible. Tries lazy lookup with HTTP HEAD on cache miss.</remarks>
        public UriTemplate GetLinkTemplate(string rel)
        {
            var template = _links.Where(x => x.Templated && x.Rel == rel)
                                 .Select(x => new UriTemplate(x.Href))
                                 .FirstOrDefault();

            if (template == null && !_defaultLinkTemplates.TryGetValue(rel, out template))
            {
                // Lazy lookup
                // NOTE: Synchronous execution so the method remains easy to use in constructors and properties
                Exception? error = null;
                Task.Run(async () =>
                {
                    try
                    {
                        await HandleAsync(() => HttpClient.HeadAsync(Uri)).NoContext();
                    }
                    catch (Exception ex)
                    {
                        error = ex;
                    }
                }).Wait();
                if (error != null)
                    throw new KeyNotFoundException($"No link template with rel={rel} provided by endpoint {Uri}.", error);

                template = _links.Where(x => x.Templated && x.Rel == rel).Select(x => new UriTemplate(x.Href)).FirstOrDefault();
                if (template == null)
                    throw new KeyNotFoundException($"No link template with rel={rel} provided by endpoint {Uri}.");
            }

            return template;
        }

        public Uri LinkTemplate(string rel, IDictionary<string, object> variables)
            => Uri.Join(GetLinkTemplate(rel).Resolve(variables));

        public Uri LinkTemplate(string rel, object variables)
            => LinkTemplate(
                rel,
                variables.GetType().GetProperties()
                         .Where(property => property.GetGetMethod() != null && property.GetIndexParameters().Length == 0)
                         .ToDictionary(property => property.Name, property => property.GetValue(variables, null)));

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

        public override string ToString()
            => GetType().Name + ": " + Uri;

        /// <summary>
        /// Returns a constructor for <typeparamref name="TEndpoint"/> as a function with a referrer an a relative URI as input.
        /// </summary>
        /// <exception cref="ArgumentException">No suitable constructor found on <typeparamref name="TEndpoint"/>.</exception>
        protected static Func<IEndpoint, Uri, TEndpoint> GetConstructor<TEndpoint>()
            where TEndpoint : IEndpoint
        {
            var type = typeof(TEndpoint);

            var constructor = TypeExtensions.GetConstructor<IEndpoint, Uri, TEndpoint>();
            if (constructor != null) return constructor;

            var altConstructor = TypeExtensions.GetConstructor<IEndpoint, string, TEndpoint>();
            if (altConstructor == null)
                throw new ArgumentException($"{type} must have a public constructor with an {nameof(IEndpoint)} and a {nameof(Uri)} or string parameter.", nameof(type));

            return (endpoint, relativeUri) => altConstructor(endpoint, relativeUri.OriginalString);
        }
    }
}
