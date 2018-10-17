using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command operating on an <see cref="IActionEndpoint"/>.
    /// </summary>
    public class ActionCommand : EndpointCommand<IActionEndpoint>
    {
        /// <summary>
        /// Creates a new REST action command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public ActionCommand(IActionEndpoint endpoint)
            : base(endpoint)
        {}

        protected override async Task ExecuteInnerAsync(IReadOnlyList<string> args,
                                                        CancellationToken cancellationToken = default)
            => await Endpoint.TriggerAsync(cancellationToken);
    }

    /// <summary>
    /// Command operating on an <see cref="IActionEndpoint"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <see cref="IActionEndpoint{TEntity}"/> takes as input.</typeparam>
    public class ActionCommand<TEntity> : EndpointCommand<IActionEndpoint<TEntity>>
    {
        /// <summary>
        /// Creates a new REST action command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public ActionCommand(IActionEndpoint<TEntity> endpoint)
            : base(endpoint)
        {}

        protected override async Task ExecuteInnerAsync(IReadOnlyList<string> args,
                                                        CancellationToken cancellationToken = default)
            => await Endpoint.TriggerAsync(InputEntity(args.Skip(1).ToList()), cancellationToken);

        /// <summary>
        /// Acquires a <typeparamref name="TEntity"/> from the user, e.g. by parsing the <paramref name="args"/> or via JSON on the command-line.
        /// </summary>
        protected virtual TEntity InputEntity(IReadOnlyList<string> args)
            => JsonConvert.DeserializeObject<TEntity>((args.Count == 0) ? Console.ReadLine() : args[0]);
    }
}
