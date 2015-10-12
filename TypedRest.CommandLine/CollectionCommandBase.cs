using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Base class for building commands operating on a <typeparamref name="TEndpoint"/> using <typeparamref name="TElement"/>s.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <typeparamref name="TEndpoint"/> represents.</typeparam>
    /// <typeparam name="TEndpoint">The specific type of <see cref="IRestCollection{TEntity,TElement}"/> to operate on.</typeparam>
    /// <typeparam name="TElement">The specific type of <see cref="IRestElement{TEntity}"/>s the <typeparamref name="TEndpoint"/> provides for individual <typeparamref name="TEntity"/>s.</typeparam>
    public class CollectionCommandBase<TEntity, TEndpoint, TElement> : EndpointCommand<TEndpoint>
        where TEndpoint : IRestCollection<TEntity, TElement>
        where TElement : class, IRestElement<TEntity>
    {
        /// <summary>
        /// Creates a new REST collection command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        protected CollectionCommandBase(TEndpoint endpoint) : base(endpoint)
        {
        }

        public override async Task ExecuteAsync(IReadOnlyList<string> args)
        {
            if (args.Count == 0)
            {
                OutputEntities(await Endpoint.ReadAllAsync());
            }
            else if (args[0].ToLowerInvariant() == "create")
            {
                var newEntity = InputEntity(args.Skip(1).ToList());
                var newEndpoint = await Endpoint.CreateAsync(newEntity);
                Console.WriteLine(newEndpoint.Uri);
            }
            else
            {
                var subCommand = GetSubCommand(args[0]);
                await subCommand.ExecuteAsync(args.Skip(1).ToList());
            }
        }

        /// <summary>
        /// Creates a sub-<see cref="IEndpointCommand"/> based on the given <paramref name="id"/>, usually a sub-type of <see cref="ElementCommand{TEntity}"/>.
        /// </summary>
        protected virtual IEndpointCommand GetSubCommand(object id)
        {
            return new ElementCommand<TEntity>(Endpoint[id]);
        }

        /// <summary>
        /// Aquires a <typeparamref name="TEntity"/> from the user, e.g. by parsing the <paramref name="args"/> or via JSON on the command-line.
        /// </summary>
        protected virtual TEntity InputEntity(IReadOnlyList<string> args)
        {
            return JsonConvert.DeserializeObject<TEntity>((args.Count == 0) ? Console.ReadLine() : args[0]);
        }

        /// <summary>
        /// Outputs a collection of <typeparamref name="TEntity"/> to the user, e.g. via IDs on the command-line.
        /// </summary>
        protected virtual void OutputEntities(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                Console.WriteLine(entity.ToString());
        }
    }
}