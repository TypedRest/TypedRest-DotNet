using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TypedRest.Commands
{
    /// <summary>
    /// Base class for building commands operating on a <typeparamref name="TEndpoint"/> using <typeparamref name="TElement"/>s.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <typeparamref name="TEndpoint"/> represents.</typeparam>
    /// <typeparam name="TElement">The specific type of <see cref="IRestElement{TEntity}"/>s the <typeparamref name="TEndpoint"/> provides for individual <typeparamref name="TEntity"/>s.</typeparam>
    /// <typeparam name="TEndpoint">The specific type of <see cref="RestSetBase{TElement,TEntity}"/> to operate on.</typeparam>
    public class SetCommandBase<TEntity, TElement, TEndpoint> : EndpointCommand
        where TElement : class, IRestElement<TEntity>
        where TEndpoint : RestSetBase<TEntity, TElement>
    {
        /// <summary>
        /// The REST endpoint this command operates on.
        /// </summary>
        protected new readonly TEndpoint Endpoint;

        /// <summary>
        /// Creates a new REST set command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        protected SetCommandBase(TEndpoint endpoint) : base(endpoint)
        {
            Endpoint = endpoint;
        }

        public override async Task ExecuteAsync(IReadOnlyList<string> args)
        {
            if (args.Count == 0)
            {
                OutputEntities(await Endpoint.ReadAllAsync());
            }
            else if (args[0].ToLowerInvariant() == "create")
            {
                var newResource = await Endpoint.CreateAsync(InputEntity());
                Console.WriteLine(newResource.Uri);
            }
            else
            {
                var subCommand = GetSubCommand(args[0]);
                await subCommand.ExecuteAsync(args.Skip(1).ToList());
            }
        }

        /// <summary>
        /// Creates a sub-<see cref="EndpointCommand"/> based on the given <paramref name="id"/>, usually a sub-type of <see cref="ElementCommand{TEntity}"/>.
        /// </summary>
        protected virtual EndpointCommand GetSubCommand(object id)
        {
            return new ElementCommand<TEntity>(Endpoint[id]);
        }

        /// <summary>
        /// Aquires a <typeparamref name="TEntity"/> from the user, e.g. via JSON on the command-line.
        /// </summary>
        protected virtual TEntity InputEntity()
        {
            return JsonConvert.DeserializeObject<TEntity>(Console.ReadLine());
        }

        /// <summary>
        /// Outputs a set of <typeparamref name="TEntity"/> to the user, e.g. via IDs on the command-line.
        /// </summary>
        protected virtual void OutputEntities(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                Console.WriteLine(entity.ToString());
        }
    }
}