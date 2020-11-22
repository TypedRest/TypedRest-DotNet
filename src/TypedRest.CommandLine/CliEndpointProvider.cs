using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using TypedRest.CommandLine.IO;
using TypedRest.Endpoints;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Builds <see cref="EntryEndpoint"/>s using config files, interactive authentication, OAuth tokens, etc.
    /// </summary>
    /// <typeparam name="T">The type of entry endpoint created. Must have constructors with the following signatures: (<see cref="Uri"/>, <see cref="ICredentials"/>) for HTTP Basic Auth and (<see cref="Uri"/>, <see cref="string"/>) for OAuth token.
    /// </typeparam>
    public class CliEndpointProvider<T> : EndpointProviderBase<T>
        where T : EntryEndpoint
    {
        /// <summary>
        /// The text input/output device used for user interaction.
        /// </summary>
        public IConsole Console { get; set; } = new JsonConsole();

        protected override Uri RequestUri()
        {
            while (true)
            {
                try
                {
                    return new Uri(Console.Read("Endpoint URI:"), UriKind.Absolute);
                }
                catch (UriFormatException ex)
                {
                    Console.WriteError(ex.Message);
                }
            }
        }

        protected override string RequestToken(Uri uri)
        {
            ShowTokenProvider(uri);

            return Console.ReadSecret("OAuth token:");
        }

        /// <summary>
        /// Tries to determine a website that provides tokens for <paramref name="uri"/> and opens it in the default browser.
        /// </summary>
        protected virtual void ShowTokenProvider(Uri uri)
        {
            if (!Environment.UserInteractive) return;

            var endpoint = NewEndpoint(uri, new NetworkCredential());
            try
            {
                Process.Start(endpoint.Link("token-provider").AbsoluteUri);
            }
            catch (KeyNotFoundException)
            {}
        }
    }
}
