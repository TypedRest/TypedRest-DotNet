using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command operating on a <see cref="IStreamingCollectionEndpoint{TEntity,TElementEndpoint}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    /// <typeparam name="TEndpoint">The specific type of <see cref="IStreamingCollectionEndpoint{TEntity,TElementEndpoint}"/> to operate on.</typeparam>
    /// <typeparam name="TElementEndpoint">The specific type of <see cref="IElementEndpoint{TEntity}"/> the <typeparamref name="TEndpoint"/> provides for individual <typeparamref name="TEntity"/>s.</typeparam>
    /// <typeparam name="TElementCommand">The specific type of <see cref="IEndpointCommand"/> is used to handle <typeparamref name="TElementEndpoint"/>s. This must be a non-abstract class with a constructor that takes a <typeparamref name="TElementEndpoint"/>, unless you override <c>BuildElementCommand</c>.</typeparam>
    public abstract class StreamingCollectionCommand<TEntity, TEndpoint, TElementEndpoint, TElementCommand> : CollectionCommand<TEntity, TEndpoint, TElementEndpoint, TElementCommand>
        where TEndpoint : class, IStreamingCollectionEndpoint<TEntity, TElementEndpoint>
        where TElementEndpoint : class, IEndpoint
        where TElementCommand : class, IEndpointCommand
    {
        /// <summary>
        /// Creates a new REST streaming collection command.
        /// </summary>
        /// <param name="endpoint">The endpoint this command operates on.</param>
        protected StreamingCollectionCommand(TEndpoint endpoint)
            : base(endpoint)
        {}

        public override async Task ExecuteAsync(IReadOnlyList<string> args,
                                                CancellationToken cancellationToken = default)
        {
            if (args.Count > 0 && args[0].ToLowerInvariant() == "stream")
            {
                switch (args.Count)
                {
                    case 1:
                        await StreamFromBeginning(cancellationToken);
                        return;

                    case 2:
                        await StreamFromOffset(long.Parse(args[1]), cancellationToken);
                        return;

                    default:
                        throw new ArgumentException("Unknown command: " + args[2]);
                }
            }

            await base.ExecuteAsync(args, cancellationToken);
        }

        private async Task StreamFromBeginning(CancellationToken cancellationToken)
            => await OutputEntitiesAsync(Endpoint.GetObservable(), cancellationToken);

        private async Task StreamFromOffset(long startIndex, CancellationToken cancellationToken)
            => await OutputEntitiesAsync(Endpoint.GetObservable(startIndex), cancellationToken);

        /// <summary>
        /// Outputs a stream of <typeparamref name="TEntity"/>s to the user, e.g., via <see cref="object.ToString"/> on the console.
        /// </summary>
        protected virtual async Task OutputEntitiesAsync(IObservable<TEntity> observable,
                                                         CancellationToken cancellationToken = default)
            => await new StreamPrinter<TEntity>(Console).PrintAsync(observable, cancellationToken);
    }
}
