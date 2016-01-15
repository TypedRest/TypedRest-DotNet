using System.Threading.Tasks;
using Caliburn.Micro;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// Common base class for components creating or updating elements.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the view model represents.</typeparam>
    /// <typeparam name="TEndpoint">The type of <see cref="IEndpoint"/> to operate on.</typeparam>
    public abstract class ElementViewModelBase<TEntity, TEndpoint> : EndpointViewModel<TEndpoint>
        where TEndpoint : IEndpoint
    {
        public TEntity Entity { get; protected set; }

        /// <summary>
        /// Creates a new REST element view model.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this view model operates on.</param>
        /// <param name="eventAggregator"></param>
        protected ElementViewModelBase(TEndpoint endpoint, IEventAggregator eventAggregator)
            : base(endpoint, eventAggregator)
        {
        }

        public virtual async void Save()
        {
            await WithErrorHandlingAsync(async () =>
            {
                await OnSaveAsync();
                TryClose();
            });
        }

        /// <summary>
        /// Handler for saving the input.
        /// </summary>
        protected abstract Task OnSaveAsync();
    }
}