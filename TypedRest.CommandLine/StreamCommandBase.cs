using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Base class for building commands operating on an <see cref="IStreamEndpoint{TEntity,TElementEndpoint}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <typeparamref name="TEndpoint"/> represents.</typeparam>
    /// <typeparam name="TEndpoint">The specific type of <see cref="IStreamEndpoint{TEntity,TElementEndpoint}"/> to operate on.</typeparam>
    /// <typeparam name="TElementEndpoint">The specific type of <see cref="IElementEndpoint{TEntity}"/> the <typeparamref name="TEndpoint"/> provides for individual <typeparamref name="TEntity"/>s.</typeparam>
    public abstract class StreamCommandBase<TEntity, TEndpoint, TElementEndpoint> : PagedCollectionCommandBase<TEntity, TEndpoint, TElementEndpoint>
        where TEndpoint : IStreamEndpoint<TEntity, TElementEndpoint>
        where TElementEndpoint : class, IElementEndpoint<TEntity>
    {
        /// <summary>
        /// Creates a new REST stream command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        protected StreamCommandBase(TEndpoint endpoint) : base(endpoint)
        {
        }

        public override async Task ExecuteAsync(IReadOnlyList<string> args,
            CancellationToken cancellationToken = default(CancellationToken))
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
        {
            var stream = Endpoint.GetStream();
            await OutputEntitiesAsync(stream, cancellationToken);
        }

        private async Task StreamFromOffset(long startIndex, CancellationToken cancellationToken)
        {
            var stream = Endpoint.GetStream(startIndex);
            await OutputEntitiesAsync(stream, cancellationToken);
        }

        /// <summary>
        /// Outputs a stream of <typeparamref name="TEntity"/>s to the user, e.g., via <see cref="object.ToString"/> on the command-line.
        /// </summary>
        protected virtual async Task OutputEntitiesAsync(IObservable<TEntity> observable,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var printer = new StreamPrinter();
            await printer.PrintAsync(observable, cancellationToken);
        }

        private class StreamPrinter : IObserver<TEntity>
        {
            private readonly TaskCompletionSource<bool> _quitEvent = new TaskCompletionSource<bool>();

            public async Task PrintAsync(IObservable<TEntity> observable, CancellationToken cancellationToken)
            {
                using (cancellationToken.Register(() => _quitEvent.SetResult(true)))
                using (observable.Subscribe(this))
                    await _quitEvent.Task;
            }

            public void OnNext(TEntity value)
            {
                Console.WriteLine(value.ToString());
            }

            public void OnError(Exception error)
            {
                Console.Error.WriteLine(error.Message);
                _quitEvent.SetResult(true);
            }

            public void OnCompleted()
            {
                _quitEvent.SetResult(true);
            }
        }
    }
}