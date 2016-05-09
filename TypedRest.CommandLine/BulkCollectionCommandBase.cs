using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Base class for building commands operating on an <see cref="IBulkCollectionEndpoint{TEntity,TElementEndpoint}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <typeparamref name="TEndpoint"/> represents.</typeparam>
    /// <typeparam name="TEndpoint">The specific type of <see cref="IBulkCollectionEndpoint{TEntity,TElementEndpoint}"/> to operate on.</typeparam>
    /// <typeparam name="TElementEndpoint">The specific type of <see cref="IElementEndpoint{TEntity}"/> the <typeparamref name="TEndpoint"/> provides for individual <typeparamref name="TEntity"/>s.</typeparam>
    public abstract class BulkCollectionCommandBase<TEntity, TEndpoint, TElementEndpoint> :
        CollectionCommandBase<TEntity, TEndpoint, TElementEndpoint>
        where TEndpoint : IBulkCollectionEndpoint<TEntity, TElementEndpoint>
        where TElementEndpoint : class, IElementEndpoint<TEntity>
    {
        /// <summary>
        /// Creates a new REST Bulk collection command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        protected BulkCollectionCommandBase(TEndpoint endpoint) : base(endpoint)
        {
        }

        public override async Task ExecuteAsync(IReadOnlyList<string> args,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (args.Count != 0)
            {
                switch (args[0].ToLowerInvariant())
                {
                    case "set-bulk":
                        await Endpoint.SetAllAsync(InputEntities(args.Skip(1).ToList()), cancellationToken);
                        return;

                    case "create-bulk":
                        await Endpoint.CreateAsync(InputEntities(args.Skip(1).ToList()), cancellationToken);
                        return;
                }
            }

            await base.ExecuteAsync(args, cancellationToken);
        }

        /// <summary>
        /// Aquires a <typeparamref name="TEntity"/> from the user, e.g. by parsing the <paramref name="args"/> or via JSON on the command-line.
        /// </summary>
        protected virtual IEnumerable<TEntity> InputEntities(IReadOnlyList<string> args)
        {
            return JsonConvert.DeserializeObject<List<TEntity>>((args.Count == 0) ? Console.ReadLine() : args[0]);
        }
    }
}