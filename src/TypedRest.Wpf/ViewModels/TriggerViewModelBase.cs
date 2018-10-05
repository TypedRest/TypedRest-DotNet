using System;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// Base class for building view models operating on an <see cref="ITriggerEndpoint"/>.
    /// </summary>
    /// <typeparam name="TEndpoint">The specific type of <see cref="ITriggerEndpoint"/> to operate on.</typeparam>
    public abstract class TriggerViewModelBase<TEndpoint> : EndpointViewModelBase<TEndpoint>
        where TEndpoint : class, ITriggerEndpoint
    {
        /// <summary>
        /// Creates a new REST trigger view model.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this view model operates on.</param>
        /// <param name="eventAggregator">Used to send refresh notifications.</param>
        protected TriggerViewModelBase(TEndpoint endpoint, IEventAggregator eventAggregator)
            : base(endpoint, eventAggregator)
        {}

        public bool CanTrigger { get; set; }

        protected override async Task OnLoadAsync()
        {
            try
            {
                await Endpoint.ProbeAsync(CancellationToken);
            }
            catch (Exception)
            {
                // HTTP OPTIONS server-side implementation is optional
            }

            CanTrigger = Endpoint.TriggerAllowed.GetValueOrDefault(CanTrigger);
            NotifyOfPropertyChange(() => CanTrigger);
        }

        public abstract void Trigger();
    }
}
