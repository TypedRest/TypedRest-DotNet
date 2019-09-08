using System;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// Base class for building view models that create or update elements.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the view model represents.</typeparam>
    /// <typeparam name="TEndpoint">The type of <see cref="IEndpoint"/> to operate on.</typeparam>
    public abstract class ElementViewModelBase<TEntity, TEndpoint> : EndpointViewModelBase<TEndpoint>
        where TEndpoint : class, IEndpoint
    {
        public TEntity Entity { get; protected set; }

        /// <summary>
        /// Creates a new REST element view model.
        /// </summary>
        /// <param name="endpoint">The endpoint this view model operates on.</param>
        /// <param name="eventAggregator"></param>
        protected ElementViewModelBase(TEndpoint endpoint, IEventAggregator eventAggregator)
            : base(endpoint, eventAggregator)
        {}

        public virtual async void Save()
            => await WithErrorHandlingAsync(async () =>
            {
                try
                {
                    await OnSaveAsync();
                    TryClose();
                }
                catch (InvalidOperationException ex)
                {
                    // This usually indicates a "lost update"
                    string question = ex.Message + "\nDo you want to refresh this page loosing any changes you have made?";
                    if (MessageBox.Show(question, "Refresh element", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        await RefreshAsync();
                }
            });

        /// <summary>
        /// Handler for saving the input.
        /// </summary>
        protected abstract Task OnSaveAsync();
    }
}
