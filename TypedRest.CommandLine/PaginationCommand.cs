using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command operating on an <see cref="IPaginationEndpoint{TElement}"/>.
    /// </summary>
    /// <typeparam name="TElement">The type of element the <see cref="IPaginationEndpoint{TElement}"/> represents.</typeparam>
    public class PaginationCommand<TElement> : EndpointCommand<IPaginationEndpoint<TElement>>
    {
        /// <summary>
        /// Creates a new REST pagination command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public PaginationCommand(IPaginationEndpoint<TElement> endpoint) : base(endpoint)
        {
        }

        public override async Task ExecuteAsync(IReadOnlyList<string> args,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            switch (args.Count)
            {
                case 0:
                    await ReadAll(cancellationToken);
                    return;

                case 1:
                    await ReadHeadOrTail(args, cancellationToken);
                    return;

                case 2:
                    await ReadSubset(args, cancellationToken);
                    return;

                default:
                    throw new ArgumentException("Unknown command: " + args[2]);
            }
        }

        private async Task ReadAll(CancellationToken cancellationToken)
        {
            var elements = await Endpoint.ReadAllAsync(cancellationToken);
            OutputElements(elements);
        }

        private async Task ReadHeadOrTail(IReadOnlyList<string> args, CancellationToken cancellationToken)
        {
            long index = long.Parse(args[0]);
            var range = (index >= 0)
                ? new RangeItemHeaderValue(from: index, to: null)
                : new RangeItemHeaderValue(from: null, to: -index);
            var elements = await Endpoint.ReadPartialAsync(range, cancellationToken);
            OutputElements(elements.Elements);
        }

        private async Task ReadSubset(IReadOnlyList<string> args, CancellationToken cancellationToken)
        {
            var range = new RangeItemHeaderValue(from: long.Parse(args[0]), to: long.Parse(args[1]));
            var elements = await Endpoint.ReadPartialAsync(range, cancellationToken);
            OutputElements(elements.Elements);
        }

        /// <summary>
        /// Outputs a collection of <typeparamref name="TElement"/>s to the user, e.g., via <see cref="object.ToString"/> on the command-line.
        /// </summary>
        protected virtual void OutputElements(IEnumerable<TElement> elements)
        {
            foreach (var element in elements)
                Console.WriteLine(element.ToString());
        }
    }
}