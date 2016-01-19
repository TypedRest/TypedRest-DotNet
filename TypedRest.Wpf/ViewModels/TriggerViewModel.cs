using System;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using TypedRest.Wpf.Events;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// View model operating on an <see cref="ITriggerEndpoint"/>.
    /// </summary>
    public class TriggerViewModel<TEndpoint> : EndpointViewModelBase<TEndpoint>
        where TEndpoint : ITriggerEndpoint
    {
        /// <summary>
        /// Creates a new REST trigger view model.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this view model operates on.</param>
        /// <param name="eventAggregator">Used to send refresh notifications.</param>
        /// <param name="caption">A caption for the triggerable action.</param>
        public TriggerViewModel(TEndpoint endpoint, IEventAggregator eventAggregator, string caption)
            : base(endpoint, eventAggregator)
        {
            DisplayName = caption;
        }

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

        public async void Trigger()
        {
            await WithErrorHandlingAsync(async () =>
            {
                await OnTriggerAsync();
                EventAggregator.Publish(new TriggerEvent<TEndpoint>(Endpoint), null);
                MessageBox.Show("Successful.", DisplayName, MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        private async Task OnTriggerAsync()
        {
            await Endpoint.TriggerAsync(CancellationToken);
        }
    }

    /// <summary>
    /// View model operating on an <see cref="ITriggerEndpoint"/>.
    /// </summary>
    public class TriggerViewModel : TriggerViewModel<ITriggerEndpoint>
    {
        /// <summary>
        /// Creates a new REST trigger view model.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this view model operates on.</param>
        /// <param name="eventAggregator">Used to send refresh notifications.</param>
        /// <param name="caption">A caption for the triggerable action.</param>
        public TriggerViewModel(ITriggerEndpoint endpoint, IEventAggregator eventAggregator, string caption)
            : base(endpoint, eventAggregator, caption)
        {
        }
    }
}