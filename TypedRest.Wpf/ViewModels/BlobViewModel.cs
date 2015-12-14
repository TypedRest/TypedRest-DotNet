using Caliburn.Micro;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// View model operating on an <see cref="IBlobEndpoint"/>.
    /// </summary>
    public class BlobViewModel : EndpointViewModel<IBlobEndpoint>
    {
        /// <summary>
        /// Creates a new REST blob view model.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this view model operates on.</param>
        /// <param name="eventAggregator">Used to send refresh notifications.</param>
        /// <param name="caption">A caption for the blob.</param>
        public BlobViewModel(IBlobEndpoint endpoint, IEventAggregator eventAggregator, string caption)
            : base(endpoint, eventAggregator)
        {
            DisplayName = caption;
        }
    }
}