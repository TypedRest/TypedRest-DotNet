using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command operating on a <see cref="IStreamEndpoint{TEntity,TElementEndpoint}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    /// <typeparam name="TEndpoint">The specific type of <see cref="IStreamEndpoint{TEntity,TElementEndpoint}"/> to operate on.</typeparam>
    /// <typeparam name="TElementEndpoint">The specific type of <see cref="IElementEndpoint{TEntity}"/> the <typeparamref name="TEndpoint"/> provides for individual <typeparamref name="TEntity"/>s.</typeparam>
    /// <typeparam name="TElementCommand">The specific type of <see cref="IEndpointCommand"/> is used to handle <typeparamref name="TElementEndpoint"/>s. This must be a non-abstract class with a constructor that takes a <typeparamref name="TElementEndpoint"/>, unless you override <see cref="CollectionCommand{TEntity,TEndpoint,TElementEndpoint,TElementCommand}.BuildElementCommand"/>.</typeparam>
    public abstract class StreamCommand<TEntity, TEndpoint, TElementEndpoint, TElementCommand> : CollectionCommand<TEntity, TEndpoint, TElementEndpoint, TElementCommand>
        where TEndpoint : class, IStreamEndpoint<TEntity, TElementEndpoint>
        where TElementEndpoint : class, IEndpoint
        where TElementCommand : class, IEndpointCommand
    {
        /// <summary>
        /// Creates a new REST stream command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public StreamCommand(TEndpoint endpoint) : base(endpoint)
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
            var printer = new StreamPrinter<TEntity>();
            await printer.PrintAsync(observable, cancellationToken);
        }
    }

    /// <summary>
    /// Command operating on a <see cref="IStreamEndpoint{TEntity,TElementEndpoint}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    /// <typeparam name="TElementEndpoint">The specific type of <see cref="IElementEndpoint{TEntity}"/> the endpoint provides for individual <typeparamref name="TEntity"/>s.</typeparam>
    /// <typeparam name="TElementCommand">The specific type of <see cref="IEndpointCommand"/> is used to handle <typeparamref name="TElementEndpoint"/>s. This must be a non-abstract class with a constructor that takes a <typeparamref name="TElementEndpoint"/>, unless you override <see cref="CollectionCommand{TEntity,TEndpoint,TElementEndpoint,TElementCommand}.BuildElementCommand"/>.</typeparam>
    public class StreamCommand<TEntity, TElementEndpoint, TElementCommand> : StreamCommand<TEntity, IStreamEndpoint<TEntity, TElementEndpoint>, TElementEndpoint, TElementCommand>
        where TElementEndpoint : class, IEndpoint
        where TElementCommand : class, IEndpointCommand
    {
        /// <summary>
        /// Creates a new REST stream command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public StreamCommand(IStreamEndpoint<TEntity, TElementEndpoint> endpoint) : base(endpoint)
        {
        }
    }

    /// <summary>
    /// Command operating on a <see cref="IStreamEndpoint{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    public class StreamCommand<TEntity> : StreamCommand<TEntity, IElementEndpoint<TEntity>, ElementCommand<TEntity>>
    {
        /// <summary>
        /// Creates a new REST stream command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public StreamCommand(IStreamEndpoint<TEntity> endpoint) : base(endpoint)
        {
        }

        protected override ElementCommand<TEntity> BuildElementCommand(IElementEndpoint<TEntity> elementEndpoint) => new ElementCommand<TEntity>(elementEndpoint);
    }
}