using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command operating on an <see cref="IPollingEndpoint{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <see cref="IPollingEndpoint{TEntity}"/> represents.</typeparam>
    public class PollingCommand<TEntity> : ElementCommand<TEntity>
    {
        protected new readonly IPollingEndpoint<TEntity> Endpoint;

        /// <summary>
        /// Creates a new REST polling command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public PollingCommand(IPollingEndpoint<TEntity> endpoint)
            : base(endpoint)
        {
            Endpoint = endpoint;
        }

        protected override async Task ExecuteInnerAsync(IReadOnlyList<string> args,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (args.Count >= 1 && args[0].ToLowerInvariant() == "poll")
            {
                var interval = (args.Count >= 2) ? TimeSpan.Parse(args[1]) : TimeSpan.FromSeconds(2);
                var stream = Endpoint.GetStream(interval);
                await OutputEntitiesAsync(stream, cancellationToken);
                return;
            }

            await base.ExecuteInnerAsync(args, cancellationToken);
        }

        /// <summary>
        /// Outputs a stream of <typeparamref name="TEntity"/>s to the user, e.g., via <see cref="object.ToString"/> on the command-line.
        /// </summary>
        protected virtual async Task OutputEntitiesAsync(IObservable<TEntity> observable,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var printer = new StreamPrinter<TEntity>();
            await printer.PrintAsync(observable, cancellationToken);
        }
    }
}