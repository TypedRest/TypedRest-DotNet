using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Base class for building commands operating on an <see cref="IPagedCollectionEndpoint{TEntity,TElement}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <typeparamref name="TEndpoint"/> represents.</typeparam>
    /// <typeparam name="TEndpoint">The specific type of <see cref="IPagedCollectionEndpoint{TEntity,TElement}"/> to operate on.</typeparam>
    /// <typeparam name="TElement">The specific type of <see cref="IElementEndpoint{TEntity}"/> the <typeparamref name="TEndpoint"/> provides for individual <typeparamref name="TEntity"/>s.</typeparam>
    public abstract class PagedCollectionCommandBase<TEntity, TEndpoint, TElement> :
        CollectionCommandBase<TEntity, TEndpoint, TElement>
        where TEndpoint : IPagedCollectionEndpoint<TEntity, TElement>
        where TElement : class, IElementEndpoint<TEntity>
    {
        /// <summary>
        /// Creates a new REST paged collection command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        protected PagedCollectionCommandBase(TEndpoint endpoint) : base(endpoint)
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
}