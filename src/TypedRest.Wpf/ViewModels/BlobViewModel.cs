using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using TypedRest.Endpoints.Raw;
using TypedRest.Wpf.Events;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// View model operating on an <see cref="IBlobEndpoint"/>.
    /// </summary>
    public class BlobViewModel : EndpointViewModelBase<IBlobEndpoint>
    {
        /// <summary>
        /// Creates a new REST blob view model.
        /// </summary>
        /// <param name="endpoint">The endpoint this view model operates on.</param>
        /// <param name="eventAggregator">Used to send refresh notifications.</param>
        /// <param name="caption">A caption for the blob.</param>
        public BlobViewModel(IBlobEndpoint endpoint, IEventAggregator eventAggregator, string caption)
            : base(endpoint, eventAggregator)
        {
            DisplayName = caption;
        }

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

            CanDownload = Endpoint.DownloadAllowed.GetValueOrDefault(CanDownload);
            NotifyOfPropertyChange(() => CanDownload);

            CanUpload = Endpoint.UploadAllowed.GetValueOrDefault(CanUpload);
            NotifyOfPropertyChange(() => CanUpload);
        }

        public virtual async void Upload() => await WithErrorHandlingAsync(async () =>
        {
            await Endpoint.UploadFromAsync(null);
            EventAggregator.Publish(new BlobUploadEvent(Endpoint), null);
        });

        /// <summary>
        /// Controls whether a download button is shown.
        /// </summary>
        public bool CanDownload { get; set; }

        /// <summary>
        /// Controls whether an upload button is shown.
        /// </summary>
        public bool CanUpload { get; set; }
    }
}
