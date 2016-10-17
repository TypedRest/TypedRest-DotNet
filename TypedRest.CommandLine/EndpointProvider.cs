using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Builds <see cref="EntryEndpoint"/>s using config files, interactive authentication, OAuth tokens, etc.
    /// </summary>
    /// <typeparam name="T">The type of entry endpoint created. Must have constructors with the following signatures: (<see cref="Uri"/>, <see cref="ICredentials"/>) for HTTP Basic Auth and (<see cref="Uri"/>, <see cref="string"/>) for OAuth token.
    /// </typeparam>
    public class EndpointProvider<T> : EndpointProviderBase<T>
        where T : EntryEndpoint
    {
        protected override Uri RequestUri()
        {
            Uri uri = null;
            while (uri == null)
            {
                Console.Write("Endpoint URI: ");
                string input = Console.ReadLine();
                if (input == null) return null;
                try
                {
                    uri = new Uri(input, UriKind.Absolute);
                }
                catch (UriFormatException ex)
                {
                    Console.Error.WriteLine(ex.Message);
                }
            }
            return uri;
        }

        protected override string RequestToken(Uri uri)
        {
            ShowTokenProvider(uri);

            // Increase maximum length for Console.ReadLine()
            var defaultReader = Console.In;
            byte[] inputBuffer = new byte[1024];
            Stream inputStream = Console.OpenStandardInput(inputBuffer.Length);
            Console.SetIn(new StreamReader(inputStream, Console.InputEncoding, false, inputBuffer.Length));

            try
            {
                Console.Write("OAuth token: ");
                return Console.ReadLine();
            }
            finally
            {
                Console.SetIn(defaultReader);
            }
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
            {
            }
        }
    }
}