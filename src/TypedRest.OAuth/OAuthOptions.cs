using System;

namespace TypedRest
{
    /// <summary>
    /// Options for OAuth 2.0 / OpenID Connect authentication.
    /// </summary>
    public class OAuthOptions
    {
        /// <summary>
        /// The URI of the identity server to request an authentication token from.
        /// </summary>
        public Uri Uri { get; set; } = default!;

        /// <summary>
        /// The client identifier to present to the identity server.
        /// </summary>
        public string ClientId { get; set; } = default!;

        /// <summary>
        /// The client secret to present to the identity server.
        /// </summary>
        public string ClientSecret { get; set; } = default!;

        /// <summary>
        /// The scope to request an authentication token for.
        /// </summary>
        public string? Scope { get; set; }

        /// <summary>
        /// The audience to request an authentication token for.
        /// </summary>
        public string? Audience { get; set; }
    }
}
