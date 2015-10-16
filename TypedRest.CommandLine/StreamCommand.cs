using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command operating on an <see cref="IStreamEndpoint{TElement}"/>.
    /// </summary>
    /// <typeparam name="TElement">The type of element the <see cref="IStreamEndpoint{TElement}"/> represents.</typeparam>
    public class StreamCommand<TElement> : PaginationCommand<TElement>
    {
        /// <summary>
        /// The REST endpoint this command operates on.
        /// </summary>
        protected new readonly IStreamEndpoint<TElement> Endpoint;

        /// <summary>
        /// Creates a new REST stream command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public StreamCommand(IStreamEndpoint<TElement> endpoint) : base(endpoint)
        {
            Endpoint = endpoint;
        }

        protected override async Task ExecuteInnerAsync(IReadOnlyList<string> args,
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
                        await StreamFromOffset(args, cancellationToken);
                        return;

                    default:
                        throw new ArgumentException("Unknown command: " + args[2]);
                }
            }

            await base.ExecuteInnerAsync(args, cancellationToken);
        }

        private async Task StreamFromBeginning(CancellationToken cancellationToken)
        {
            var stream = Endpoint.GetStream();
            await OutputEntitiesAsync(stream, cancellationToken);
        }

        private async Task StreamFromOffset(IReadOnlyList<string> args, CancellationToken cancellationToken)
        {
            long startIndex = long.Parse(args[1]);
            var stream = Endpoint.GetStream(startIndex);
            await OutputEntitiesAsync(stream, cancellationToken);
        }

        /// <summary>
        /// Outputs a stream of <typeparamref name="TElement"/>s to the user, e.g., via <see cref="object.ToString"/> on the command-line.
        /// </summary>
        protected virtual async Task OutputEntitiesAsync(IObservable<TElement> observable,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var printer = new StreamPrinter();
            await printer.PrintAsync(observable, cancellationToken);
        }

        private class StreamPrinter : IObserver<TElement>
        {
            private readonly TaskCompletionSource<bool> _quitEvent = new TaskCompletionSource<bool>();

            public async Task PrintAsync(IObservable<TElement> observable, CancellationToken cancellationToken)
            {
                using (cancellationToken.Register(() => _quitEvent.SetResult(true)))
                using (observable.Subscribe(this))
                    await _quitEvent.Task;
            }

            public void OnNext(TElement value)
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