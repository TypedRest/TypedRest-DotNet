using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command operating on a <see cref="IStreamingEndpoint{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    /// <typeparam name="TEndpoint">The specific type of <see cref="IStreamingEndpoint{TEntity}"/> to operate on.</typeparam>
    public abstract class StreamingCommand<TEntity, TEndpoint> : EndpointCommand<TEndpoint>
        where TEndpoint : class, IStreamingEndpoint<TEntity>
    {
        /// <summary>
        /// Creates a new REST streaming command.
        /// </summary>
        /// <param name="endpoint">The endpoint this command operates on.</param>
        protected StreamingCommand(TEndpoint endpoint)
            : base(endpoint)
        {}

        protected override async Task ExecuteInnerAsync(IReadOnlyList<string> args, CancellationToken cancellationToken = default)
            => await OutputEntitiesAsync(Endpoint.GetObservable(), cancellationToken);

        /// <summary>
        /// Outputs a stream of <typeparamref name="TEntity"/>s to the user, e.g., via <see cref="object.ToString"/> on the console.
        /// </summary>
        protected virtual async Task OutputEntitiesAsync(IObservable<TEntity> observable, CancellationToken cancellationToken = default)
            => await new StreamPrinter<TEntity>(Console).PrintAsync(observable, cancellationToken);
    }
}
