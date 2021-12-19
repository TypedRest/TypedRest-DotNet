using System.Reflection;
using TypedRest.Endpoints;

namespace TypedRest.CommandLine;

/// <summary>
/// Builds <see cref="EntryEndpoint"/>s using config files, interactive authentication, OAuth tokens, etc.
/// </summary>
/// <typeparam name="T">The type of entry endpoint to be created. Must have a constructor with the following signature: (<see cref="Uri"/>)</typeparam>
public abstract class EndpointProviderBase<T> : IEndpointProvider<T>
    where T : EntryEndpoint
{
    protected virtual string ConfigDir => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        Assembly.GetEntryAssembly()?.GetName().Name ?? "TypedRest");

    private string UriFile => Path.Combine(ConfigDir, "uri");

    /// <summary>
    /// Gets an URI and stores it.
    /// </summary>
    protected virtual Uri GetUri()
    {
        var uri = GetLocalUri() ?? GetStoredUri();
        if (uri != null) return uri;

        uri = RequestUri();
        if (uri == null) throw new InvalidOperationException("Unable to request endpoint URI.");

        if (!Directory.Exists(ConfigDir)) Directory.CreateDirectory(ConfigDir);
        File.WriteAllText(UriFile, uri.AbsoluteUri, Encoding.UTF8);
        return uri;
    }

    /// <summary>
    /// Gets an endpoint URI placed in a file next to the executable.
    /// </summary>
    /// <returns>The stored URI or <c>null</c> if none exists.</returns>
    private static Uri? GetLocalUri()
    {
        try
        {
            string localUriFile = Path.Combine(
                Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? Directory.GetCurrentDirectory(),
                "uri");
            return File.Exists(localUriFile)
                ? new Uri(File.ReadAllText(localUriFile, Encoding.UTF8), UriKind.Absolute)
                : null;
        }
        catch (UriFormatException)
        {
            return null;
        }
    }

    /// <summary>
    /// Gets a previously stored endpoint URI.
    /// </summary>
    /// <returns>The stored URI or <c>null</c> if none exists.</returns>
    private Uri? GetStoredUri()
    {
        try
        {
            return File.Exists(UriFile)
                ? new Uri(File.ReadAllText(UriFile, Encoding.UTF8), UriKind.Absolute)
                : null;
        }
        catch (UriFormatException)
        {
            return null;
        }
    }

    /// <summary>
    /// Asks the user or a service for the base URI of the REST API.
    /// </summary>
    /// <returns>The endpoint URI or <c>null</c> if it cannot be requested.</returns>
    protected abstract Uri? RequestUri();

    protected virtual string TokenCacheFile => Path.Combine(ConfigDir, "token");

    /// <summary>
    /// Gets an OAuth token and caches it.
    /// </summary>
    private string GetToken(Uri uri)
    {
        string? token = GetCachedToken();
        if (token != null) return token;

        token = RequestToken(uri);
        if (token == null) throw new InvalidOperationException("Unable to request OAuth token.");

        if (!Directory.Exists(ConfigDir)) Directory.CreateDirectory(ConfigDir);
        File.WriteAllText(TokenCacheFile, token, Encoding.UTF8);

        return token;
    }

    /// <summary>
    /// Gets a previously cached OAuth token.
    /// </summary>
    /// <returns>The cached token or <c>null</c> if none exists.</returns>
    private string? GetCachedToken()
        => File.Exists(TokenCacheFile)
            ? File.ReadAllText(TokenCacheFile, Encoding.UTF8)
            : null;

    /// <summary>
    /// Asks the user or a service for the OAuth token to present as a "Bearer" to the REST API.
    /// </summary>
    /// <returns>The OAuth token or <c>null</c> if it cannot be requested.</returns>
    protected abstract string? RequestToken(Uri uri);

    public void ResetAuthentication()
    {
        if (File.Exists(TokenCacheFile)) File.Delete(TokenCacheFile);
        if (!string.IsNullOrEmpty(GetStoredUri()?.UserInfo)) File.Delete(UriFile);
    }

    public T Build()
    {
        var endpoint = NewEndpoint(GetUri());

        if (string.IsNullOrEmpty(endpoint.Uri.UserInfo))
        {
            string token = GetToken(endpoint.Uri);
            endpoint.HttpClient.DefaultRequestHeaders.Authorization = new("Bearer", token);
        }

        return endpoint;
    }

    /// <summary>
    /// Instantiates a <typeparamref name="T"/>.
    /// </summary>
    /// <param name="uri">The base URI of the REST API.</param>
    protected virtual T NewEndpoint(Uri uri)
        => Activator.CreateInstance(typeof(T), uri) as T
        ?? throw new MissingMethodException($"Unable to find matching constructor {typeof(T).Name}({nameof(Uri)}).");
}