using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using TypedRest.Wpf.Events;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// View model for showing or updating an existing elemented represented by a <see cref="IElementEndpoint{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to represent.</typeparam>
    public class ElementViewModel<TEntity> : ElementViewModelBase<TEntity, IElementEndpoint<TEntity>>
    {
        /// <summary>
        /// Creates a new REST element view model.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this view model operates on.</param>
        /// <param name="eventAggregator">Used to send refresh notifications.</param>
        public ElementViewModel(IElementEndpoint<TEntity> endpoint, IEventAggregator eventAggregator)
            : base(endpoint, eventAggregator)
        {
        }

        protected override async Task OnLoadAsync()
        {
            Entity = await Endpoint.ReadAsync(CancellationToken);
            DisplayName = Entity.ToString();
            NotifyOfPropertyChange(() => Entity);

            CanSave = Endpoint.UpdateAllowed.GetValueOrDefault(CanSave);
            NotifyOfPropertyChange(() => CanSave);

            CanDelete = Endpoint.DeleteAllowed.GetValueOrDefault(CanDelete);
            NotifyOfPropertyChange(() => CanDelete);
        }

        /// <summary>
        /// Controls whether a save button is shown and fields are editable.
        /// </summary>
        public bool CanSave { get; set; }

        protected override async Task OnSaveAsync()
        {
            await Endpoint.UpdateAsync(Entity, CancellationToken);
            EventAggregator.Publish(new ElementUpdatedEvent<TEntity>(Endpoint), null);
        }

        /// <summary>
        /// Controls whether a delete button is shown.
        /// </summary>
        public bool CanDelete { get; set; }

        /// <summary>
        /// Delete all selected elements.
        /// </summary>
        public virtual async void Delete()
        {
            string question = $"Are you sure you want to delete {DisplayName}?";
            if (MessageBox.Show(question, "Delete element", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                await WithErrorHandlingAsync(async () =>
                {
                    await OnDeleteAsync();
                    TryClose();
                });
            }
        }

        /// <summary>
        /// Handler for deleting the element.
        /// </summary>
        protected virtual async Task OnDeleteAsync()
        {
            await Endpoint.DeleteAsync(CancellationToken);
            EventAggregator.Publish(new ElementDeletedEvent<TEntity>(Endpoint), null);
        }
    }
}