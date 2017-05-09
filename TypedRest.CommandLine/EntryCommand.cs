using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command providing an entry point to a hierachy of named <see cref="IEndpointCommand"/>s.
    /// </summary>
    /// <typeparamref name="TEndpoint">The specific type of <see cref="IEndpoint"/> the command starts with.</typeparamref>
    public class EntryCommand<TEndpoint> : EndpointCommand<TEndpoint>, IEnumerable<KeyValuePair<string, Func<TEndpoint, IEndpointCommand>>>
        where TEndpoint : class, IEndpoint
    {
        /// <summary>
        /// Creates a new REST entry command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public EntryCommand(TEndpoint endpoint) : base(endpoint)
        {
        }

        private readonly Dictionary<string, Func<TEndpoint, IEndpointCommand>> _commandProviders =
            new Dictionary<string, Func<TEndpoint, IEndpointCommand>>(StringComparer.OrdinalIgnoreCase);

        #region Enumerable
        public IEnumerator<KeyValuePair<string, Func<TEndpoint, IEndpointCommand>>> GetEnumerator()
        {
            return _commandProviders.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _commandProviders.GetEnumerator();
        }
        #endregion

        public void Add(string name, Func<TEndpoint, IEndpointCommand> commandProvider)
        {
            _commandProviders.Add(name, commandProvider);
        }

        protected override IEndpointCommand GetSubCommand(string name)
        {
            Func<TEndpoint, IEndpointCommand> commandProvider;
            return _commandProviders.TryGetValue(name, out commandProvider) ? commandProvider(Endpoint) : null;
        }

        protected override Task ExecuteInnerAsync(IReadOnlyList<string> args,
            CancellationToken cancellationToken = new CancellationToken())
        {
            Console.Error.WriteLine("Known commands:");
            foreach (string name in _commandProviders.Keys)
                Console.Error.WriteLine(name);

            return base.ExecuteInnerAsync(args, cancellationToken);
        }
    }
}