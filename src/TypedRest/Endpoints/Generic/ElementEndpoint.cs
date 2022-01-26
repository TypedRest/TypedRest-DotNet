using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json;

namespace TypedRest.Endpoints.Generic;

/// <summary>
/// Endpoint for an individual resource.
/// </summary>
/// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
public class ElementEndpoint<TEntity> : CachingEndpointBase, IElementEndpoint<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Creates a new element endpoint.
    /// </summary>
    /// <param name="referrer">The endpoint used to navigate to this one.</param>
    /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s.</param>
    public ElementEndpoint(IEndpoint referrer, Uri relativeUri)
        : base(referrer, relativeUri)
    {}

    /// <summary>
    /// Creates a new element endpoint.
    /// </summary>
    /// <param name="referrer">The endpoint used to navigate to this one.</param>
    /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Add a <c>./</c> prefix here to imply a trailing slash <paramref name="referrer"/>'s URI.</param>
    public ElementEndpoint(IEndpoint referrer, string relativeUri)
        : base(referrer, relativeUri)
    {}

    public TEntity? Response
        => ResponseCache?.GetContent().ReadAsAsync<TEntity>(Serializer).Result;

    public virtual async Task<TEntity> ReadAsync(CancellationToken cancellationToken = default)
    {
        var content = await GetContentAsync(cancellationToken);
        return await content.ReadAsAsync<TEntity>(Serializer, cancellationToken).NoContext();
    }

    public async Task<bool> ExistsAsync(CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.HeadAsync(Uri, cancellationToken).NoContext();
        if (response.IsSuccessStatusCode) return true;
        if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.Gone) return false;

        await ErrorHandler.HandleAsync(response).NoContext();
        return false;
    }

    public bool? SetAllowed => IsMethodAllowed(HttpMethod.Put);

    public virtual async Task<TEntity?> SetAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        using var content = new ObjectContent<TEntity>(entity, Serializer);
        using var response = await PutContentAsync(content, cancellationToken);
        return await TryReadAsAsync(response, cancellationToken);
    }

    public bool? MergeAllowed => IsMethodAllowed(HttpMethods.Patch);

    public async Task<TEntity?> MergeAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        using var response = await HandleAsync(() => HttpClient.PatchAsync(Uri, entity, Serializer, cancellationToken)).NoContext();
        ResponseCache = null;
        return await TryReadAsAsync(response, cancellationToken);
    }

    public bool? DeleteAllowed => IsMethodAllowed(HttpMethod.Delete);

    public virtual async Task DeleteAsync(CancellationToken cancellationToken = default)
        => await DeleteContentAsync(cancellationToken);

    public async Task<TEntity?> UpdateAsync(Action<TEntity> updateAction, int maxRetries = 3, CancellationToken cancellationToken = default)
    {
        using var activity = StartActivity();

        int retryCounter = 0;
        while (true)
        {
            var entity = await ReadAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            updateAction(entity);
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                return await SetAsync(entity, cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                if (retryCounter++ >= maxRetries) throw;
                await ex.HttpRetryDelayAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
            }
        }
    }

    public async Task<TEntity?> UpdateAsync(Action<JsonPatchDocument<TEntity>> patchAction, int maxRetries = 3, CancellationToken cancellationToken = default)
    {
        if (Serializer is not JsonMediaTypeFormatter serializer)
            throw new NotSupportedException($"JSON Patch can only be used if the endpoint's serializer is a {nameof(JsonMediaTypeFormatter)}.");

        using var activity = StartActivity();

        var patch = new JsonPatchDocument<TEntity>(new List<Operation<TEntity>>(), serializer.SerializerSettings.ContractResolver);
        patchAction(patch);

        var response = await HttpClient.SendAsync(new(HttpMethods.Patch, Uri)
        {
            Content = new StringContent(JsonConvert.SerializeObject(patch))
            {
                Headers = {ContentType = new("application/json-patch+json")}
            }
        }, cancellationToken).NoContext();

        if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.MethodNotAllowed)
            return await UpdateAsync(patch.ApplyTo, maxRetries, cancellationToken);

        await ErrorHandler.HandleAsync(response).NoContext();

        return await TryReadAsAsync(response, cancellationToken);
    }

    private async Task<TEntity?> TryReadAsAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            return await response.Content.ReadAsAsync<TEntity?>(Serializer, cancellationToken).NoContext();
        }
        catch (UnsupportedMediaTypeException)
        {
            return null;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}