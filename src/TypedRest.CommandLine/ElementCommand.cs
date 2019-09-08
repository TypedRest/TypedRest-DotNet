using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command operating on an <see cref="IElementEndpoint{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    public class ElementCommand<TEntity> : EndpointCommand<IElementEndpoint<TEntity>>
    {
        /// <summary>
        /// Creates a new REST element command.
        /// </summary>
        /// <param name="endpoint">The endpoint this command operates on.</param>
        public ElementCommand(IElementEndpoint<TEntity> endpoint)
            : base(endpoint)
        {}

        protected override async Task ExecuteInnerAsync(IReadOnlyList<string> args, CancellationToken cancellationToken = default)
        {
            if (args.Count == 0)
            {
                OutputEntity(await Endpoint.ReadAsync(cancellationToken));
                return;
            }

            switch (args[0].ToLowerInvariant())
            {
                case "set":
                case "update": // deprecated
                {
                    var updatedEntity = InputEntity(args.Skip(1).ToList());
                    var result = await Endpoint.SetAsync(updatedEntity, cancellationToken);
                    if (result != null) OutputEntity(result);
                    break;
                }

                case "merge": // deprecated
                {
                    var partialEntity = InputEntity(args.Skip(1).ToList());
                    var result = await Endpoint.MergeAsync(partialEntity, cancellationToken);
                    if (result != null) OutputEntity(result);
                    break;
                }

                case "delete":
                    await Endpoint.DeleteAsync(cancellationToken);
                    break;

                default:
                    await base.ExecuteInnerAsync(args, cancellationToken);
                    break;
            }
        }

        /// <summary>
        /// Acquires a <typeparamref name="TEntity"/> from the user, e.g. by parsing the <paramref name="args"/> or via JSON on the console.
        /// </summary>
        protected virtual TEntity InputEntity(IReadOnlyList<string> args)
            => (args.Count == 0) ? Console.Read<TEntity>() : JsonConvert.DeserializeObject<TEntity>(args[0]);

        /// <summary>
        /// Outputs a <typeparamref name="TEntity"/> to the user via the console.
        /// </summary>
        protected virtual void OutputEntity(TEntity entity)
            => Console.Write(entity);
    }
}
