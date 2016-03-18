using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command operating on an <see cref="IFunctionEndpoint{TResult}"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of entity the <see cref="IFunctionEndpoint{TResult}"/> returns as a result.</typeparam>
    public class FunctionCommand<TResult> : EndpointCommand<IFunctionEndpoint<TResult>>
    {
        /// <summary>
        /// Creates a new REST function command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public FunctionCommand(IFunctionEndpoint<TResult> endpoint) : base(endpoint)
        {
        }

        protected override async Task ExecuteInnerAsync(IReadOnlyList<string> args,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            OutputEntity(await Endpoint.TriggerAsync(cancellationToken));
        }

        /// <summary>
        /// Outputs a <typeparamref name="TResult"/> to the user, e.g. via JSON on the command-line.
        /// </summary>
        protected virtual void OutputEntity(TResult entity)
        {
            Console.WriteLine(JsonConvert.SerializeObject(entity));
        }
    }

    /// <summary>
    /// Command operating on an <see cref="IFunctionEndpoint{TEntity,TResult}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <see cref="IFunctionEndpoint{TEntity,TResult}"/> takes as input.</typeparam>
    /// <typeparam name="TResult">The type of entity the <see cref="IFunctionEndpoint{TEntity,TResult}"/> returns as a result.</typeparam>
    public class FunctionCommand<TEntity, TResult> : EndpointCommand<IFunctionEndpoint<TEntity, TResult>>
    {
        /// <summary>
        /// Creates a new REST function command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public FunctionCommand(IFunctionEndpoint<TEntity, TResult> endpoint) : base(endpoint)
        {
        }

        protected override async Task ExecuteInnerAsync(IReadOnlyList<string> args,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            OutputEntity(await Endpoint.TriggerAsync(InputEntity(args.Skip(1).ToList()), cancellationToken));
        }

        /// <summary>
        /// Aquires a <typeparamref name="TEntity"/> from the user, e.g. by parsing the <paramref name="args"/> or via JSON on the command-line.
        /// </summary>
        protected virtual TEntity InputEntity(IReadOnlyList<string> args)
        {
            return JsonConvert.DeserializeObject<TEntity>((args.Count == 0) ? Console.ReadLine() : args[0]);
        }

        /// <summary>
        /// Outputs a <typeparamref name="TResult"/> to the user, e.g. via JSON on the command-line.
        /// </summary>
        protected virtual void OutputEntity(TResult entity)
        {
            Console.WriteLine(JsonConvert.SerializeObject(entity));
        }
    }
}