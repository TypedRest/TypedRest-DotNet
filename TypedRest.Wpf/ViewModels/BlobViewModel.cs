namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// View model operating on an <see cref="IBlobEndpoint"/>.
    /// </summary>
    public class BlobViewModel : ViewModelBase<IBlobEndpoint>
    {
        /// <summary>
        /// Creates a new REST blob view model.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this view model operates on.</param>
        public BlobViewModel(IBlobEndpoint endpoint) : base(endpoint)
        {
        }
    }
}