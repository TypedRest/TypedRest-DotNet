using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using TypedRest.Wpf.Events;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// View model operating on an <see cref="IActionEndpoint"/>.
    /// </summary>
    public class ActionViewModel<TEndpoint> : RpcViewModelBase<TEndpoint>
        where TEndpoint : class, IActionEndpoint
    {
        /// <summary>
        /// Creates a new REST action view model.
        /// </summary>
        /// <param name="endpoint">The endpoint this view model operates on.</param>
        /// <param name="eventAggregator">Used to send refresh notifications.</param>
        /// <param name="caption">A caption for the invokable action.</param>
        public ActionViewModel(TEndpoint endpoint, IEventAggregator eventAggregator, string caption)
            : base(endpoint, eventAggregator)
        {
            DisplayName = caption;
        }

        public override async void Invoke() => await WithErrorHandlingAsync(async () =>
        {
            await OnInvokeAsync();
            EventAggregator.Publish(new InvokeEvent(Endpoint), null);
            MessageBox.Show("Successful.", DisplayName, MessageBoxButton.OK, MessageBoxImage.Information);
        });

        private async Task OnInvokeAsync() => await Endpoint.InvokeAsync(CancellationToken);
    }
}
