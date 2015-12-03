using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// Base class for building view models operating on an <see cref="ICollectionEndpoint{TEntity,TElementEndpoint}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <typeparamref name="TEndpoint"/> represents.</typeparam>
    /// <typeparam name="TEndpoint">The specific type of <see cref="ICollectionEndpoint{TEntity,TElementEndpoint}"/> to operate on.</typeparam>
    /// <typeparam name="TElementEndpoint">The specific type of <see cref="IElementEndpoint{TEntity}"/> the <typeparamref name="TEndpoint"/> provides for individual <typeparamref name="TEntity"/>s.</typeparam>
    public abstract class CollectionViewModelBase<TEntity, TEndpoint, TElementEndpoint> : EndpointViewModel<TEndpoint>
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
        public bool CanOpenElement { get; set; }

        /// <summary>
        /// Creates a new REST collection view model.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this view model operates on.</param>
        protected CollectionViewModelBase(TEndpoint endpoint) : base(endpoint)
        {
            DisplayName = typeof (TEntity).Name;
        }

        public ICollection<TEntity> Elements { get; private set; }

        public ICollection<TEntity> SelectedElements { get; set; }

        protected override async Task OnLoadAync()
        {
            Elements = await Endpoint.ReadAllAsync(CancellationToken);
            NotifyOfPropertyChange(() => Elements);
        }

        /// <summary>
        /// Handler for opening an existing element in the collection.
        /// </summary>
        protected virtual void OnOpenElement(TEntity entity)
        {
            ((IConductor)Parent).ActivateItem(BuildElementScreen(Endpoint[entity]));
        }

        /// <summary>
        /// Builds a sub-<see cref="IScreen"/> for viewing or editing an existing <typeparamref name="TEntity"/> represented by the given <paramref name="elementEndpoint"/>.
        /// </summary>
        protected abstract IScreen BuildElementScreen(TElementEndpoint elementEndpoint);

        /// <summary>
        /// Handler for deleting all selected elements.
        /// </summary>
        public virtual async Task OnDeleteElements()
        {
            string message = "Are you sure you want to delete the following elements?" +
                             SelectedElements.Select(x => x.ToString())
                                 .Aggregate((workingSet, entity) => workingSet + "\n" + entity);

            if (MessageBox.Show(message, "Delete elements", MessageBoxButton.YesNo, MessageBoxImage.Warning) ==
                MessageBoxResult.Yes)
            {
                await WithErrorHandlingAsync(async () =>
                {
                    foreach (var entity in SelectedElements)
                        await Endpoint[entity].DeleteAsync();
                });
            }
        }

        /// <summary>
        /// Handler for creating a new element in the collection.
        /// </summary>
        public virtual void OnCreateElement()
        {
            ((IConductor)Parent).ActivateItem(BuildCreateElementScreen());
        }

        /// <summary>
        /// Builds a sub-<see cref="IScreen"/> for creating a new <typeparamref name="TEntity"/> in the collection endpoint.
        /// </summary>
        protected abstract IScreen BuildCreateElementScreen();
    }
}