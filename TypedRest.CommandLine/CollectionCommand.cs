using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command operating on a <see cref="ICollectionEndpoint{TEntity,TElementEndpoint}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    /// <typeparam name="TEndpoint">The specific type of <see cref="ICollectionEndpoint{TEntity,TElementEndpoint}"/> to operate on.</typeparam>
    /// <typeparam name="TElementEndpoint">The specific type of <see cref="IElementEndpoint{TEntity}"/> the <typeparamref name="TEndpoint"/> provides for individual <typeparamref name="TEntity"/>s.</typeparam>
    /// <typeparam name="TElementCommand">The specific type of <see cref="IEndpointCommand"/> is used to handle <typeparamref name="TElementEndpoint"/>s. This must be a non-abstract class with a constructor that takes a <typeparamref name="TElementEndpoint"/>, unless you override <see cref="BuildElementCommand"/>.</typeparam>
    public abstract class CollectionCommand<TEntity, TEndpoint, TElementEndpoint, TElementCommand> : EndpointCommand<TEndpoint>
        where TEndpoint : ICollectionEndpoint<TEntity, TElementEndpoint>
        where TElementEndpoint : class, IEndpoint
        where TElementCommand : class, IEndpointCommand
    {
        /// <summary>
        /// Creates a new REST collection command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public CollectionCommand(TEndpoint endpoint) : base(endpoint)
        {
        }

        public override async Task ExecuteAsync(IReadOnlyList<string> args,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (args.Count == 0)
            {
                OutputEntities(await Endpoint.ReadAllAsync(cancellationToken));
                return;
            }

            switch (args[0].ToLowerInvariant())
            {
                case "create":
                    var newEntity = InputEntity(args.Skip(1).ToList());
                    var newEndpoint = await Endpoint.CreateAsync(newEntity, cancellationToken);
                    if (newEndpoint != null)
                        await BuildElementCommand(newEndpoint).ExecuteAsync(new string[0], cancellationToken);
                    return;
            }

            await base.ExecuteAsync(args, cancellationToken);
        }

        protected override IEndpointCommand GetSubCommand(string name)
        {
            return BuildElementCommand(Endpoint[name]);
        }

        /// <summary>
        /// Builds an <see cref="IEndpointCommand"/> for the given <paramref name="elementEndpoint"/>.
        /// </summary>
        protected virtual TElementCommand BuildElementCommand(TElementEndpoint elementEndpoint) => (TElementCommand)Activator.CreateInstance(typeof(TElementCommand), elementEndpoint);

        /// <summary>
        /// Aquires a <typeparamref name="TEntity"/> from the user, e.g. by parsing the <paramref name="args"/> or via JSON on the command-line.
        /// </summary>
        protected virtual TEntity InputEntity(IReadOnlyList<string> args)
        {
            return JsonConvert.DeserializeObject<TEntity>((args.Count == 0) ? Console.ReadLine() : args[0]);
        }

        /// <summary>
        /// Outputs a collection of <typeparamref name="TEntity"/>s to the user, e.g., via <see cref="object.ToString"/> on the command-line.
        /// </summary>
        protected virtual void OutputEntities(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                Console.WriteLine(entity.ToString());
        }
    }

    /// <summary>
    /// Command operating on a <see cref="ICollectionEndpoint{TEntity,TElementEndpoint}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    /// <typeparam name="TElementEndpoint">The specific type of <see cref="IElementEndpoint{TEntity}"/> the endpoint provides for individual <typeparamref name="TEntity"/>s.</typeparam>
    /// <typeparam name="TElementCommand">The specific type of <see cref="IEndpointCommand"/> is used to handle <typeparamref name="TElementEndpoint"/>s. This must be a non-abstract class with a constructor that takes a <typeparamref name="TElementEndpoint"/>, unless you override <see cref="CollectionCommand{TEntity,TEndpoint,TElementEndpoint,TElementCommand}.BuildElementCommand"/>.</typeparam>
    public class CollectionCommand<TEntity, TElementEndpoint, TElementCommand> : CollectionCommand<TEntity, ICollectionEndpoint<TEntity, TElementEndpoint>, TElementEndpoint, TElementCommand>
        where TElementEndpoint : class, IEndpoint
        where TElementCommand : class, IEndpointCommand
    {
        /// <summary>
        /// Creates a new REST collection command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public CollectionCommand(ICollectionEndpoint<TEntity, TElementEndpoint> endpoint) : base(endpoint)
        {
        }
    }

    /// <summary>
    /// Command operating on a <see cref="ICollectionEndpoint{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    public class CollectionCommand<TEntity> : CollectionCommand<TEntity, IElementEndpoint<TEntity>, ElementCommand<TEntity>>
    {
        /// <summary>
        /// Creates a new REST collection command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public CollectionCommand(ICollectionEndpoint<TEntity> endpoint) : base(endpoint)
        {
        }

        protected override ElementCommand<TEntity> BuildElementCommand(IElementEndpoint<TEntity> elementEndpoint) => new ElementCommand<TEntity>(elementEndpoint);
    }
}