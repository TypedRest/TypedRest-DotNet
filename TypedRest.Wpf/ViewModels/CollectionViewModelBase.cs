using System.Collections.Generic;
using Caliburn.Micro;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// Base class for building view models operating on an <see cref="ICollectionEndpoint{TEntity,TElementEndpoint}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <typeparamref name="TEndpoint"/> represents.</typeparam>
    /// <typeparam name="TEndpoint">The specific type of <see cref="ICollectionEndpoint{TEntity,TElementEndpoint}"/> to operate on.</typeparam>
    /// <typeparam name="TElementEndpoint">The specific type of <see cref="IElementEndpoint{TEntity}"/> the <typeparamref name="TEndpoint"/> provides for individual <typeparamref name="TEntity"/>s.</typeparam>
    public abstract class CollectionViewModelBase<TEntity, TEndpoint, TElementEndpoint> : ViewModelBase<TEndpoint>
        where TEndpoint : ICollectionEndpoint<TEntity, TElementEndpoint>
        where TElementEndpoint : class, IElementEndpoint<TEntity>
    {
        /// <summary>
        /// Creates a new REST collection view model.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this view model operates on.</param>
        protected CollectionViewModelBase(TEndpoint endpoint) : base(endpoint)
        {
        }

        public ICollection<TEntity> Elements { get; private set; }

        public TEntity SelectedElement { get; set; }

        protected override async void OnActivate()
        {
            base.OnActivate();

            Elements = await Endpoint.ReadAllAsync(CancellationToken);
            NotifyOfPropertyChange(() => Elements);
        }

        /// <summary>
        /// Creates a sub-<see cref="IScreen"/> for the given <paramref name="elementEndpoint"/>.
        /// </summary>
        protected abstract IScreen GetElementScreen(TElementEndpoint elementEndpoint);
    }
}