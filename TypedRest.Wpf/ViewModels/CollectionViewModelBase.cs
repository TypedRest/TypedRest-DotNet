using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        /// Controls whether an update button is shown.
        /// </summary>
        public bool CanCreate { get; set; }

        /// <summary>
        /// Controls whether a delete button is shown.
        /// </summary>
        public bool CanDelete { get; set; }

        /// <summary>
        /// Controls whether selecting individual elements opens an edit view.
        /// </summary>
        public bool CanUpdate { get; set; }

        /// <summary>
        /// Creates a new REST collection view model.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this view model operates on.</param>
        protected CollectionViewModelBase(TEndpoint endpoint) : base(endpoint)
        {
            DisplayName = typeof (TEntity).Name;
        }

        public ICollection<TEntity> Elements { get; private set; }

        public TEntity SelectedElement { get; set; }

        protected override async Task OnLoad()
        {
            Elements = await Endpoint.ReadAllAsync(CancellationToken);
            NotifyOfPropertyChange(() => Elements);
        }

        /// <summary>
        /// Handler for creating a new entity.
        /// </summary>
        protected virtual Task OnCreate()
        {
            var screen = BuildCreateElementScreen();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a sub-<see cref="IScreen"/> for creating a new <typeparamref name="TEntity"/> in the collection endpoint.
        /// </summary>
        protected abstract IScreen BuildCreateElementScreen();

        /// <summary>
        /// Handler for updating an existing <paramref name="entity"/>.
        /// </summary>
        protected virtual Task OnUpdate(TEntity entity)
        {
            var screen = BuildUpdateElementScreen(Endpoint[entity]);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a sub-<see cref="IScreen"/> for editing an existing <typeparamref name="TEntity"/> represented by the given <paramref name="elementEndpoint"/>.
        /// </summary>
        protected abstract IScreen BuildUpdateElementScreen(TElementEndpoint elementEndpoint);

        /// <summary>
        /// Handler for deleting a set of existing <paramref name="entities"/>.
        /// </summary>
        protected virtual Task OnDelete(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }
    }
}