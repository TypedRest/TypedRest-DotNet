using System.Threading.Tasks;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// Common base class for view models operating on individual entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the view model represents.</typeparam>
    /// <typeparam name="TEndpoint">The type of <see cref="IEndpoint"/> to operate on.</typeparam>
    public abstract class ElementViewModelBase<TEntity, TEndpoint> : EndpointViewModel<TEndpoint>
        where TEndpoint : IEndpoint
    {
        public TEntity Entity { get; protected set; }

        /// <summary>
        /// Creates a new REST element view model.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this view model operates on.</param>
        protected ElementViewModelBase(TEndpoint endpoint) : base(endpoint)
        {
        }

        /// <summary>
        /// Handler for saving the input.
        /// </summary>
        protected abstract Task OnSave();
    }
}