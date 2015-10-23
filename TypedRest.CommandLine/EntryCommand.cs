using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command providing an entry point to a hierachy of <see cref="IEndpoint"/>s.
    /// </summary>
    /// <typeparamref name="TEndpoint">The specific type of <see cref="IEndpoint"/> the command starts with.</typeparamref>
    public class EntryCommand<TEndpoint> : CommandBase<TEndpoint>, IEnumerable<KeyValuePair<string, Func<TEndpoint, ICommand>>>
        where TEndpoint : IEndpoint
    {
        /// <summary>
        /// Creates a new REST entry command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public EntryCommand(TEndpoint endpoint) : base(endpoint)
        {
        }

        private readonly Dictionary<string, Func<TEndpoint, ICommand>> _commandProviders =
            new Dictionary<string, Func<TEndpoint, ICommand>>(StringComparer.InvariantCultureIgnoreCase);

        #region Enumerable
        public IEnumerator<KeyValuePair<string, Func<TEndpoint, ICommand>>> GetEnumerator()
        {
            return _commandProviders.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _commandProviders.GetEnumerator();
        }
        #endregion

        public void Add(string name, Func<TEndpoint, ICommand> commandProvider)
        {
            _commandProviders.Add(name, commandProvider);
        }

        protected override ICommand GetSubCommand(string name)
        {
            Func<TEndpoint, ICommand> commandProvider;
            return _commandProviders.TryGetValue(name, out commandProvider) ? commandProvider(Endpoint) : null;
        }

        protected override async Task ExecuteInnerAsync(IReadOnlyList<string> args,
            CancellationToken cancellationToken = new CancellationToken())
        {
            Console.Error.WriteLine("Known commands:");
            foreach (string name in _commandProviders.Keys)
                Console.Error.WriteLine(name);

            await base.ExecuteInnerAsync(args, cancellationToken);
        }
    }
}