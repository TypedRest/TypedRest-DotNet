using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command operating on a <see cref="IPagedCollectionEndpoint{TEntity,TElementEndpoint}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <typeparamref name="TEndpoint"/> represents.</typeparam>
    /// <typeparam name="TEndpoint">The specific type of <see cref="IPagedCollectionEndpoint{TEntity,TElementEndpoint}"/> to operate on.</typeparam>
    /// <typeparam name="TElementEndpoint">The specific type of <see cref="IElementEndpoint{TEntity}"/> the <typeparamref name="TEndpoint"/> provides for individual <typeparamref name="TEntity"/>s.</typeparam>
    /// <typeparam name="TElementCommand">The specific type of <see cref="IEndpointCommand"/> is used to handle <typeparamref name="TElementEndpoint"/>s. This must be a non-abstract class with a constructor that takes a <typeparamref name="TElementEndpoint"/>, unless you override <see cref="CollectionCommand{TEntity,TEndpoint,TElementEndpoint,TElementCommand}.BuildElementCommand"/>.</typeparam>
    public abstract class PagedCollectionCommand<TEntity, TEndpoint, TElementEndpoint, TElementCommand> : CollectionCommand<TEntity, TEndpoint, TElementEndpoint, TElementCommand>
        where TEndpoint : class, IPagedCollectionEndpoint<TEntity, TElementEndpoint>
        where TElementEndpoint : class, IEndpoint
        where TElementCommand : class, IEndpointCommand
    {
        /// <summary>
        /// Creates a new REST paged collection command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public PagedCollectionCommand(TEndpoint endpoint) : base(endpoint)
        {
        }

        public override async Task ExecuteAsync(IReadOnlyList<string> args,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (args.Count == 1)
            {
                var range = GetRange(args[0]);
                if (range != null)
                {
                    var elements = await Endpoint.ReadRangeAsync(range, cancellationToken);
                    OutputEntities(elements.Elements);
                    return;
                }
            }

            await base.ExecuteAsync(args, cancellationToken);
        }

        private static RangeItemHeaderValue GetRange(string input)
        {
            var parts = input.Split('-');
            if (parts.Length != 2) return null;

            long fromOut, toOut;
            long? from = null, to = null;

            if (long.TryParse(parts[0], out fromOut)) from = fromOut;
            if (long.TryParse(parts[1], out toOut)) to = toOut;

            return new RangeItemHeaderValue(from, to);
        }
    }

    /// <summary>
    /// Command operating on a <see cref="IPagedCollectionEndpoint{TEntity,TElementEndpoint}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    /// <typeparam name="TElementEndpoint">The specific type of <see cref="IElementEndpoint{TEntity}"/> the endpoint provides for individual <typeparamref name="TEntity"/>s.</typeparam>
    /// <typeparam name="TElementCommand">The specific type of <see cref="IEndpointCommand"/> is used to handle <typeparamref name="TElementEndpoint"/>s. This must be a non-abstract class with a constructor that takes a <typeparamref name="TElementEndpoint"/>, unless you override <see cref="CollectionCommand{TEntity,TEndpoint,TElementEndpoint,TElementCommand}.BuildElementCommand"/>.</typeparam>
    public class PagedCollectionCommand<TEntity, TElementEndpoint, TElementCommand> : PagedCollectionCommand<TEntity, IPagedCollectionEndpoint<TEntity, TElementEndpoint>, TElementEndpoint, TElementCommand>
        where TElementEndpoint : class, IEndpoint
        where TElementCommand : class, IEndpointCommand
    {
        /// <summary>
        /// Creates a new REST paged collection command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public PagedCollectionCommand(IPagedCollectionEndpoint<TEntity, TElementEndpoint> endpoint) : base(endpoint)
        {
        }
    }

    /// <summary>
    /// Command operating on a <see cref="IPagedCollectionEndpoint{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <see cref="IPagedCollectionEndpoint{TEntity}"/> represents.</typeparam>
    public class PagedCollectionCommand<TEntity> : PagedCollectionCommand<TEntity, IElementEndpoint<TEntity>, ElementCommand<TEntity>>
    {
        /// <summary>
        /// Creates a new REST paged collection command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public PagedCollectionCommand(IPagedCollectionEndpoint<TEntity> endpoint) : base(endpoint)
        {
        }

        protected override ElementCommand<TEntity> BuildElementCommand(IElementEndpoint<TEntity> elementEndpoint) => new ElementCommand<TEntity>(elementEndpoint);
    }
}