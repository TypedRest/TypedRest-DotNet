using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command providing an entry point to a hierarchy of named <see cref="IEndpointCommand"/>s.
    /// </summary>
    /// <typeparamref name="TEndpoint">The specific type of <see cref="IEndpoint"/> the command starts with.</typeparamref>
    public class EntryCommand<TEndpoint> : EndpointCommand<TEndpoint>, IEnumerable<KeyValuePair<string, Func<TEndpoint, IEndpointCommand>>>
        where TEndpoint : class, IEndpoint
    {
        /// <summary>
        /// Creates a new REST entry command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public EntryCommand(TEndpoint endpoint)
            : base(endpoint)
        {}

        private readonly Dictionary<string, Func<TEndpoint, IEndpointCommand>> _commandProviders =
            new Dictionary<string, Func<TEndpoint, IEndpointCommand>>(StringComparer.OrdinalIgnoreCase);

        #region Enumerable
        public IEnumerator<KeyValuePair<string, Func<TEndpoint, IEndpointCommand>>> GetEnumerator() => _commandProviders.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _commandProviders.GetEnumerator();
        #endregion

        public void Add(string name, Func<TEndpoint, IEndpointCommand> commandProvider) => _commandProviders.Add(name, commandProvider);

        protected override IEndpointCommand GetSubCommand(string name)
            => _commandProviders.TryGetValue(name, out var commandProvider) ? commandProvider(Endpoint) : null;

        protected override Task ExecuteInnerAsync(IReadOnlyList<string> args,
                                                  CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteError("Known commands:" + Environment.NewLine + string.Join(Environment.NewLine, _commandProviders.Keys));

            return base.ExecuteInnerAsync(args, cancellationToken);
        }
    }
}
