using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command operating on a <see cref="IBulkCollectionEndpoint{TEntity,TElementEndpoint}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    /// <typeparam name="TEndpoint">The specific type of <see cref="IBulkCollectionEndpoint{TEntity,TElementEndpoint}"/> to operate on.</typeparam>
    /// <typeparam name="TElementEndpoint">The specific type of <see cref="IElementEndpoint{TEntity}"/> the <typeparamref name="TEndpoint"/> provides for individual <typeparamref name="TEntity"/>s.</typeparam>
    /// <typeparam name="TElementCommand">The specific type of <see cref="IEndpointCommand"/> is used to handle <typeparamref name="TElementEndpoint"/>s. This must be a non-abstract class with a constructor that takes a <typeparamref name="TElementEndpoint"/>, unless you override <see cref="CollectionCommand{TEntity,TEndpoint,TElementEndpoint,TElementCommand}.BuildElementCommand"/>.</typeparam>
    public abstract class BulkCollectionCommand<TEntity, TEndpoint, TElementEndpoint, TElementCommand> : CollectionCommand<TEntity, TEndpoint, TElementEndpoint, TElementCommand>
        where TEndpoint : IBulkCollectionEndpoint<TEntity, TElementEndpoint>
        where TElementEndpoint : class, IEndpoint
        where TElementCommand : class, IEndpointCommand
    {
        /// <summary>
        /// Creates a new REST bulk collection command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public BulkCollectionCommand(TEndpoint endpoint) : base(endpoint)
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

    /// <summary>
    /// Command operating on a <see cref="IBulkCollectionEndpoint{TEntity,TElementEndpoint}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    /// <typeparam name="TElementEndpoint">The specific type of <see cref="IElementEndpoint{TEntity}"/> the endpoint provides for individual <typeparamref name="TEntity"/>s.</typeparam>
    /// <typeparam name="TElementCommand">The specific type of <see cref="IEndpointCommand"/> is used to handle <typeparamref name="TElementEndpoint"/>s. This must be a non-abstract class with a constructor that takes a <typeparamref name="TElementEndpoint"/>, unless you override <see cref="CollectionCommand{TEntity,TEndpoint,TElementEndpoint,TElementCommand}.BuildElementCommand"/>.</typeparam>
    public class BulkCollectionCommand<TEntity, TElementEndpoint, TElementCommand> : BulkCollectionCommand<TEntity, IBulkCollectionEndpoint<TEntity, TElementEndpoint>, TElementEndpoint, TElementCommand>
        where TElementEndpoint : class, IEndpoint
        where TElementCommand : class, IEndpointCommand
    {
        /// <summary>
        /// Creates a new REST bulk collection command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public BulkCollectionCommand(IBulkCollectionEndpoint<TEntity, TElementEndpoint> endpoint) : base(endpoint)
        {
        }
    }

    /// <summary>
    /// Command operating on a <see cref="BulkCollectionEndpoint{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    public class BulkCollectionCommand<TEntity> : BulkCollectionCommand<TEntity, IElementEndpoint<TEntity>, ElementCommand<TEntity>>
    {
        /// <summary>
        /// Creates a new REST bulk collection command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public BulkCollectionCommand(IBulkCollectionEndpoint<TEntity> endpoint) : base(endpoint)
        {
        }

        protected override ElementCommand<TEntity> BuildElementCommand(IElementEndpoint<TEntity> elementEndpoint) => new ElementCommand<TEntity>(elementEndpoint);
    }
}