using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TypedRest.Endpoints.Rpc;

namespace TypedRest.CommandLine.Commands.Rpc
{
    /// <summary>
    /// Command operating on an <see cref="IProducerEndpoint{TResult}"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of entity the <see cref="IProducerEndpoint{TResult}"/> returns as a result.</typeparam>
    public class ProducerCommand<TResult> : EndpointCommand<IProducerEndpoint<TResult>>
    {
        /// <summary>
        /// Creates a new REST function command.
        /// </summary>
        /// <param name="endpoint">The endpoint this command operates on.</param>
        public ProducerCommand(IProducerEndpoint<TResult> endpoint)
            : base(endpoint)
        {}

        protected override async Task ExecuteInnerAsync(IReadOnlyList<string> args, CancellationToken cancellationToken = default)
            => OutputEntity(await Endpoint.InvokeAsync(cancellationToken));

        /// <summary>
        /// Outputs a <typeparamref name="TResult"/> to the user via the console.
        /// </summary>
        protected virtual void OutputEntity(TResult entity)
            => Console.Write(entity);
    }
}
