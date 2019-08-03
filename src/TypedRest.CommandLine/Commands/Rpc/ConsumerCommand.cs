using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TypedRest.Endpoints.Rpc;

namespace TypedRest.CommandLine.Commands.Rpc
{
    /// <summary>
    /// Command operating on an <see cref="IConsumerEndpoint{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <see cref="IConsumerEndpoint{TEntity}"/> takes as input.</typeparam>
    public class ConsumerCommand<TEntity> : EndpointCommand<IConsumerEndpoint<TEntity>>
    {
        /// <summary>
        /// Creates a new REST action command.
        /// </summary>
        /// <param name="endpoint">The endpoint this command operates on.</param>
        public ConsumerCommand(IConsumerEndpoint<TEntity> endpoint)
            : base(endpoint)
        {}

        protected override async Task ExecuteInnerAsync(IReadOnlyList<string> args, CancellationToken cancellationToken = default)
            => await Endpoint.InvokeAsync(InputEntity(args.Skip(1).ToList()), cancellationToken);

        /// <summary>
        /// Acquires a <typeparamref name="TEntity"/> from the user, e.g. by parsing the <paramref name="args"/> or via JSON on the console.
        /// </summary>
        protected virtual TEntity InputEntity(IReadOnlyList<string> args)
            => (args.Count == 0) ? Console.Read<TEntity>() : JsonConvert.DeserializeObject<TEntity>(args[0]);
    }
}
