using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command operating on a <see cref="IRestElement{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <see cref="IRestElement{TEntity}"/> represents.</typeparam>
    public class ElementCommand<TEntity> : EndpointCommand
    {
        /// <summary>
        /// The REST endpoint this command operates on.
        /// </summary>
        protected readonly new IRestElement<TEntity> Endpoint;

        /// <summary>
        /// Creates a new REST element command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public ElementCommand(IRestElement<TEntity> endpoint) : base(endpoint)
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
                    var updatedEntity = InputEntity(args.Skip(1).ToList());
                    await Endpoint.UpdateAsync(updatedEntity);
                    break;

                case "delete":
                    await Endpoint.DeleteAsync();
                    break;

                default:
                    throw new ArgumentException("Unknown command: " +args[0]);
            }
        }

        /// <summary>
        /// Aquires a <typeparamref name="TEntity"/> from the user, e.g. by parsing the <paramref name="args"/> or via JSON on the command-line.
        /// </summary>
        protected virtual TEntity InputEntity(IReadOnlyList<string> args)
        {
            return JsonConvert.DeserializeObject<TEntity>((args.Count == 0) ? Console.ReadLine() : args[0]);
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