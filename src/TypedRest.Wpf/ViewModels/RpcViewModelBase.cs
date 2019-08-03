using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using TypedRest.Endpoints.Rpc;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// Base class for building view models operating on an <see cref="IRpcEndpoint"/>.
    /// </summary>
    /// <typeparam name="TEndpoint">The specific type of <see cref="IRpcEndpoint"/> to operate on.</typeparam>
    public abstract class RpcViewModelBase<TEndpoint> : EndpointViewModelBase<TEndpoint>
        where TEndpoint : class, IRpcEndpoint
    {
        /// <summary>
        /// Creates a new RPC view model.
        /// </summary>
        /// <param name="endpoint">The endpoint this view model operates on.</param>
        /// <param name="eventAggregator">Used to send refresh notifications.</param>
        protected RpcViewModelBase(TEndpoint endpoint, IEventAggregator eventAggregator)
            : base(endpoint, eventAggregator)
        {}

        public bool CanInvoke { get; set; }

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

            CanInvoke = Endpoint.InvokeAllowed.GetValueOrDefault(CanInvoke);
            NotifyOfPropertyChange(() => CanInvoke);
        }

        public abstract void Invoke();
    }
}
