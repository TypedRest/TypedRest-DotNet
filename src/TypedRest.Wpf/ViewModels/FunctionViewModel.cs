using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using TypedRest.Wpf.Events;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// View model operating on an <see cref="IFunctionEndpoint{TResult}"/>.
    /// </summary>
    public class FunctionViewModel<TEndpoint, TResult> : RpcViewModelBase<TEndpoint>
        where TEndpoint : class, IFunctionEndpoint<TResult>
    {
        /// <summary>
        /// Creates a new REST function view model.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this view model operates on.</param>
        /// <param name="eventAggregator">Used to send refresh notifications.</param>
        /// <param name="caption">A caption for the invokable function.</param>
        public FunctionViewModel(TEndpoint endpoint, IEventAggregator eventAggregator, string caption)
            : base(endpoint, eventAggregator)
        {
            DisplayName = caption;
        }

        public override async void Invoke() => await WithErrorHandlingAsync(async () =>
        {
            var result = await OnInvokeAsync();
            EventAggregator.Publish(new InvokeEvent(Endpoint), null);
            MessageBox.Show(result.ToString(), DisplayName, MessageBoxButton.OK, MessageBoxImage.Information);
        });

        private async Task<TResult> OnInvokeAsync() => await Endpoint.InvokeAsync(CancellationToken);
    }
}
