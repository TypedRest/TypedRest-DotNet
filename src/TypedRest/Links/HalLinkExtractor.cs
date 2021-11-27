using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TypedRest.Links
{
    /// <summary>
    /// Extracts links from JSON bodies according to the Hypertext Application Language (HAL) specification.
    /// </summary>
    public class HalLinkExtractor : ILinkExtractor
    {
        public async Task<IReadOnlyList<Link>> GetLinksAsync(HttpResponseMessage response)
        {
            var links = response.Content.Headers.ContentType?.MediaType switch
            {
                // ReSharper disable once RedundantSuppressNullableWarningExpression
                "application/hal+json" => ParseJsonBody(await response.Content!.ReadAsStringAsync().NoContext()),
                _ => Enumerable.Empty<Link>()
            };
            return links.ToList();
        }

        private static IEnumerable<Link> ParseJsonBody(string body)
        {
            if (string.IsNullOrEmpty(body)) yield break;

            JToken jsonBody;
            try
            {
                jsonBody = JToken.Parse(body);
            }
            catch (JsonReaderException)
            {
                yield break;
            }

            if (jsonBody.Type != JTokenType.Object) yield break;
            var linksNode = jsonBody["_links"];
            if (linksNode == null) yield break;

            foreach (var linkNode in linksNode.OfType<JProperty>())
            {
                string rel = linkNode.Name;

                switch (linkNode.Value.Type)
                {
                    case JTokenType.Array:
                        foreach (var subobj in linkNode.Value.OfType<JObject>())
                            yield return ParseLink(rel, subobj);
                        break;

                    case JTokenType.Object:
                        yield return ParseLink(rel, (JObject)linkNode.Value);
                        break;
                }
            }
        }

        private static Link ParseLink(string rel, JObject obj)
        {
            var href = obj["href"] ?? throw new FormatException("The link header is lacking the mandatory href.");
            var title = obj["title"];
            var templated = obj["templated"];
            return new(
                rel,
                href.Value<string>() ?? throw new FormatException("Link href must not be null."),
                (title?.Type == JTokenType.String) ? title.Value<string>() : null,
                templated?.Type == JTokenType.Boolean && templated.Value<bool>());
        }
    }
}
