using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TypedRest.Http;
using TypedRest.UriTemplates;

namespace TypedRest.Links
{
    /// <summary>
    /// Detects links in HTTP Link headers and HAL-like JSON bodies.
    /// </summary>
    public class DefaultLinkHandler : ILinkHandler
    {
        public async Task<(LinkDictionary links, IDictionary<string, UriTemplate> linkTemplates)> HandleAsync(HttpResponseMessage response)
        {
            Uri baseUri = response.RequestMessage?.RequestUri ?? new Uri(".");

            LinkDictionary links = new LinkDictionary();
            Dictionary<string, UriTemplate> linkTemplates = new Dictionary<string, UriTemplate>();

            void ParseLinkObject(string rel, JObject obj)
            {
                var href = obj["href"];
                if (href == null) return;

                var templated = obj["templated"];
                if (templated != null && templated.Type == JTokenType.Boolean && templated.Value<bool>())
                    linkTemplates[rel] = new UriTemplate(href.ToString());
                else
                {
                    var title = obj["title"];
                    links.Add(rel, new Link(
                        baseUri.Join(href.ToString()),
                        (title != null && title.Type == JTokenType.String) ? title.Value<string>() : null));
                }
            }

            foreach (var header in response.Headers.GetLinkHeaders().Where(x => x.Rel != null))
            {
                if (header.Templated)
                    linkTemplates[header.Rel] = new UriTemplate(header.Href);
                else
                    links.Add(header.Rel, new Link(baseUri.Join(header.Href), header.Title));
            }

            if (response.Content?.Headers.ContentType?.MediaType == "application/json")
            {
                ParseBody(
                    await response.Content.ReadAsStringAsync().NoContext(),
                    ParseLinkObject);
            }

            return (links, linkTemplates);
        }

        private static void ParseBody(string body, Action<string, JObject> objCallback)
        {
            if (string.IsNullOrEmpty(body)) return;

            JToken jsonBody;
            try
            {
                jsonBody = JToken.Parse(body);
            }
            catch (JsonReaderException)
            {
                // Unparsable bodies are handled elsewhere
                return;
            }

            if (jsonBody.Type != JTokenType.Object) return;
            var linksNode = jsonBody["_links"] ?? jsonBody["links"];
            if (linksNode == null) return;

            foreach (var linkNode in linksNode.OfType<JProperty>())
            {
                string rel = linkNode.Name;

                switch (linkNode.Value.Type)
                {
                    case JTokenType.Array:
                        foreach (var subobj in linkNode.Value.OfType<JObject>())
                            objCallback(rel, subobj);
                        break;

                    case JTokenType.Object:
                        objCallback(rel, (JObject)linkNode.Value);
                        break;
                }
            }
        }
    }
}
