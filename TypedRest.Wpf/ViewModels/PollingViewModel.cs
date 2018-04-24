using System;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// View model for tracking the state of an entity represented by a <see cref="IPollingEndpoint{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to represent.</typeparam>
    public class PollingViewModel<TEntity> : ElementViewModelBase<TEntity, IPollingEndpoint<TEntity>>
    {
        public PollingViewModel(IPollingEndpoint<TEntity> endpoint, IEventAggregator eventAggregator)
            : base(endpoint, eventAggregator)
        {}

        protected override async Task OnLoadAsync()
        {
            Entity = await Endpoint.ReadAsync(CancellationToken);
            DisplayName = Entity.ToString();
            NotifyOfPropertyChange(() => Entity);
        }

        /// <summary>
        /// Controls whether a save button is shown and fields are editable.
        /// </summary>
        public bool CanSave => false;

        protected override Task OnSaveAsync() => throw new NotImplementedException();
    }
}
