using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// Common base class for view models operating on an <see cref="IEndpoint"/>.
    /// </summary>
    /// <typeparam name="TEndpoint">The specific type of <see cref="IEndpoint"/> to operate on.</typeparam>
    public abstract class EndpointViewModel<TEndpoint> : Screen
        where TEndpoint : IEndpoint
    {
        /// <summary>
        /// The REST endpoint this view model operates on.
        /// </summary>
        protected readonly TEndpoint Endpoint;

        /// <summary>
        /// Creates a new REST endpoint view model.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this view model operates on.</param>
        protected EndpointViewModel(TEndpoint endpoint)
        {
            Endpoint = endpoint;
        }

        private CancellationTokenSource _cancellationTokenSource;

        protected CancellationToken CancellationToken => _cancellationTokenSource.Token;

        protected override async void OnActivate()
        {
            base.OnActivate();

            _cancellationTokenSource = new CancellationTokenSource();

            await RefreshAsync();
        }

        /// <summary>
        /// Reloads data from the endpoint.
        /// </summary>
        public async Task RefreshAsync()
        {
            await WithErrorHandlingAsync(OnLoadAync);
        }

        /// <summary>
        /// Handler for loading data for the endpoint.
        /// </summary>
        protected virtual Task OnLoadAync()
        {
            return Task.FromResult(true);
        }

        protected async Task WithErrorHandlingAsync(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (InvalidDataException ex)
            {
                OnError(ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                OnError(ex);
            }
            catch (KeyNotFoundException ex)
            {
                OnError(ex);
            }
            catch (InvalidOperationException ex)
            {
                OnError(ex);
            }
            catch (IndexOutOfRangeException ex)
            {
                OnError(ex);
            }
            catch (HttpRequestException ex)
            {
                OnError(ex);
            }
        }

        /// <summary>
        /// Handler for errors reported by REST endpoints.
        /// </summary>
        protected virtual void OnError(Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            _cancellationTokenSource.Cancel();
        }
    }
}