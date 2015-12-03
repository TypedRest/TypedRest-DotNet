using System.Collections.Generic;
using System.Threading.Tasks;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// A view model that can either watch another view model for refresh notifications or receive such notifications itself.
    /// </summary>
    public interface IWatcher
    {
        /// <summary>
        /// The other view models this view models is watching.
        /// </summary>
        ICollection<IWatcher> Watching { get; }

        /// <summary>
        /// The other view models that are watching this view models.
        /// </summary>
        ICollection<IWatcher> Watchers { get; }

        /// <summary>
        /// Reloads data from the endpoint.
        /// </summary>
        Task RefreshAsync();

        /// <summary>
        /// Calls <see cref="RefreshAsync"/> on all regisrered watchers and recursivley on their watchers.
        /// </summary>
        Task RefreshWatchersAsync();
    }
}