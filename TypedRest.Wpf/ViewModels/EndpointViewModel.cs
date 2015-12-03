using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// Common base class for view models operating on an <see cref="IEndpoint"/>.
    /// </summary>
    /// <typeparam name="TEndpoint">The specific type of <see cref="IEndpoint"/> to operate on.</typeparam>
    public abstract class EndpointViewModel<TEndpoint> : Screen
        where TEndpoint : IEndpoint
    {
        /// <summary>
        /// The REST endpoint this view model operates on.
        /// </summary>
        protected readonly TEndpoint Endpoint;

        /// <summary>
        /// Creates a new REST endpoint view model.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this view model operates on.</param>
        protected EndpointViewModel(TEndpoint endpoint)
        {
            Endpoint = endpoint;
        }

        private CancellationTokenSource _cancellationTokenSource;

        protected CancellationToken CancellationToken => _cancellationTokenSource.Token;

        protected override async void OnActivate()
        {
            base.OnActivate();

            _cancellationTokenSource = new CancellationTokenSource();

            // TODO: Error handling
            await OnLoad();
        }

        /// <summary>
        /// Reloads data from the endpoint.
        /// </summary>
        public async Task RefreshData()
        {
            // TODO: Error handling
            await OnLoad();
        }

        /// <summary>
        /// Handler for loading data for the endpoint.
        /// </summary>
        protected virtual Task OnLoad()
        {
            return Task.FromResult(true);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            _cancellationTokenSource.Cancel();
        }
    }
}