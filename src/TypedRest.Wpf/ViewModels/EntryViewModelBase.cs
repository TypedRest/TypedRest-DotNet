using System.Threading.Tasks;
using Caliburn.Micro;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// Base class for building view models operating on an <see cref="EntryEndpoint"/>.
    /// Use this to build a UI that provides access to an API's top-level functionality.
    /// </summary>
    /// <typeparam name="TEndpoint">The specific type of <see cref="EntryEndpoint"/> to operate on.</typeparam>
    public abstract class EntryViewModelBase<TEndpoint> : EndpointViewModelBase<TEndpoint>
        where TEndpoint : EntryEndpoint
    {
        /// <summary>
        /// Creates a new REST entry view model.
        /// </summary>
        /// <param name="endpoint">The endpoint this view model operates on.</param>
        protected EntryViewModelBase(TEndpoint endpoint)
            : base(endpoint, new EventAggregator())
        {}

        protected override async Task OnLoadAsync()
            => await Endpoint.ReadMetaAsync(CancellationToken);
    }
}
