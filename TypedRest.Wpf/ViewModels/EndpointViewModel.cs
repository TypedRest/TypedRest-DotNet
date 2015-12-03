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
    public abstract class EndpointViewModel<TEndpoint> : Screen, IWatcher
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

        public ICollection<IWatcher> Watching { get; } = new List<IWatcher>();

        public ICollection<IWatcher> Watchers { get; } = new List<IWatcher>();

        /// <summary>
        /// Starts watching another view model for refresh notifications.
        /// </summary>
        /// <param name="target">The target to watch.</param>
        protected void Watch(IWatcher target)
        {
            target.Watchers.Add(this);
            Watching.Add(target);
        }

        protected override void OnDeactivate(bool close)
        {
            // Automatically stop watching on detach
            foreach (var target in Watching)
                target.Watchers.Remove(this);
            Watching.Clear();

            // Automatically stop being watched on detach
            foreach (var watcher in Watchers)
                watcher.Watching.Remove(this);
            Watchers.Clear();

            _cancellationTokenSource.Cancel();
            base.OnDeactivate(close);
        }

        public async Task RefreshWatchersAsync()
        {
            foreach (var watcher in Watchers)
            {
                await watcher.RefreshAsync();
                await watcher.RefreshWatchersAsync();
            }
        }
    }
}