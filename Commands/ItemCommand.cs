using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TypedRest.Commands
{
    /// <summary>
    /// Command operating on a <see cref="IRestItem{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <see cref="IRestItem{TEntity}"/> represents.</typeparam>
    public class ItemCommand<TEntity> : EndpointCommand
    {
        /// <summary>
        /// The REST endpoint this command operates on.
        /// </summary>
        protected readonly new IRestItem<TEntity> Endpoint;

        /// <summary>
        /// Creates a new REST item command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public ItemCommand(IRestItem<TEntity> endpoint) : base(endpoint)
        {
            Endpoint = endpoint;
        }

        public override async Task ExecuteAsync(IReadOnlyList<string> args)
        {
            if (args.Count == 0)
            {
                OutputEntity(await Endpoint.ReadAsync());
                return;
            }

            switch (args[0].ToLowerInvariant())
            {
                case "update":
                    await Endpoint.UpdateAsync(InputEntity());
                    break;

                case "delete":
                    await Endpoint.DeleteAsync();
                    break;

                default:
                    throw new ArgumentException("Unknown argument");
            }
        }

        /// <summary>
        /// Aquires a <typeparamref name="TEntity"/> from the user, e.g. via JSON on the command-line.
        /// </summary>
        protected virtual TEntity InputEntity()
        {
            return JsonConvert.DeserializeObject<TEntity>(Console.ReadLine());
        }

        /// <summary>
        /// Outputs a <typeparamref name="TEntity"/> to the user, e.g. via JSON on the command-line.
        /// </summary>
        protected virtual void OutputEntity(TEntity entity)
        {
            Console.WriteLine(JsonConvert.SerializeObject(entity));
        }
    }
}