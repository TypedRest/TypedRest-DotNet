using System.Threading;
using Caliburn.Micro;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// View model operating on an <see cref="IEndpoint"/>.
    /// </summary>
    /// <typeparam name="TEndpoint">The specific type of <see cref="IEndpoint"/> to operate on.</typeparam>
    public abstract class ViewModelBase<TEndpoint> : Screen
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
        protected ViewModelBase(TEndpoint endpoint)
        {
            Endpoint = endpoint;
        }

        private CancellationTokenSource _cancellationTokenSource;

        protected CancellationToken CancellationToken => _cancellationTokenSource.Token;

        protected override void OnActivate()
        {
            base.OnActivate();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            _cancellationTokenSource.Cancel();
        }
    }
}