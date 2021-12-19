using System.Runtime.CompilerServices;
using TypedRest.Endpoints.Generic;

namespace TypedRest.Endpoints.Reactive;

/// <summary>
/// Endpoint for a resource that can be polled for state changes.
/// </summary>
/// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
public class PollingEndpoint<TEntity> : ElementEndpoint<TEntity>, IPollingEndpoint<TEntity>
    where TEntity : class
{
    private readonly Predicate<TEntity>? _endCondition;

    /// <summary>
    /// Creates a new polling endpoint.
    /// </summary>
    /// <param name="referrer">The endpoint used to navigate to this one.</param>
    /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s.</param>
    /// <param name="endCondition">A check to determine whether the entity has reached its final state an no further polling is required.</param>
    public PollingEndpoint(IEndpoint referrer, Uri relativeUri, Predicate<TEntity>? endCondition = null)
        : base(referrer, relativeUri)
    {
        _endCondition = endCondition;
    }

    /// <summary>
    /// Creates a new polling endpoint.
    /// </summary>
    /// <param name="referrer">The endpoint used to navigate to this one.</param>
    /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Add a <c>./</c> prefix here to imply a trailing slash <paramref name="referrer"/>'s URI.</param>
    /// <param name="endCondition">A check to determine whether the entity has reached its final state an no further polling is required.</param>
    public PollingEndpoint(IEndpoint referrer, string relativeUri, Predicate<TEntity>? endCondition = null)
        : base(referrer, relativeUri)
    {
        _endCondition = endCondition;
    }

    protected override async Task<HttpResponseMessage> HandleAsync(Func<Task<HttpResponseMessage>> request, [CallerMemberName] string caller = "unknown")
    {
        var response = await base.HandleAsync(request, caller);
        PollingInterval =
            response.Headers.RetryAfter?.Delta ??
            (response.Headers.RetryAfter?.Date - DateTime.UtcNow) ??
            PollingInterval;
        return response;
    }

    public TimeSpan PollingInterval { get; set; } = TimeSpan.FromSeconds(3);

    public IObservable<TEntity> GetObservable()
        => Observable.Create<TEntity>(async (observer, cancellationToken) =>
        {
            using var activity = StartActivity();

            TEntity previousEntity;
            try
            {
                previousEntity = await ReadAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                observer.OnError(ex);
                return;
            }
            observer.OnNext(previousEntity);

            while (_endCondition == null || !_endCondition(previousEntity))
            {
                try
                {
                    await Task.Delay(PollingInterval, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    return;
                }

                TEntity newEntity;
                try
                {
                    newEntity = await ReadAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                    return;
                }
                // ReSharper disable once RedundantSuppressNullableWarningExpression
                if (!newEntity!.Equals(previousEntity))
                    observer.OnNext(newEntity);

                previousEntity = newEntity;
            }
            observer.OnCompleted();
        });
}