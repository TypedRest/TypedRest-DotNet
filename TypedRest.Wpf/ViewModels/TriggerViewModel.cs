using System.Threading.Tasks;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// View model operating on an <see cref="ITriggerEndpoint"/>.
    /// </summary>
    public class TriggerViewModel : EndpointViewModel<ITriggerEndpoint>
    {
        /// <summary>
        /// Creates a new REST trigger view model.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this view model operates on.</param>
        /// <param name="caption">A caption for the triggerable action.</param>
        public TriggerViewModel(ITriggerEndpoint endpoint, string caption) : base(endpoint)
        {
            DisplayName = caption;
        }

        public async Task Trigger()
        {
            await Endpoint.TriggerAsync();
        }
    }
}