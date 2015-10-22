using System.Threading.Tasks;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// View model operating on an <see cref="ITriggerEndpoint"/>.
    /// </summary>
    public class TriggerViewModel : ViewModelBase<ITriggerEndpoint>
    {
        public string Caption { get; private set; }

        /// <summary>
        /// Creates a new REST trigger view model.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this view model operates on.</param>
        /// <param name="caption"></param>
        public TriggerViewModel(ITriggerEndpoint endpoint, string caption) : base(endpoint)
        {
            Caption = caption;
        }

        public async Task Trigger()
        {
            await Endpoint.TriggerAsync();
        }
    }
}