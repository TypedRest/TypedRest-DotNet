namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// View model operating on an <see cref="IElementEndpoint{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <see cref="IElementEndpoint{TEntity}"/> represents.</typeparam>
    public class ElementViewModel<TEntity> : ViewModelBase<IElementEndpoint<TEntity>>
    {
        /// <summary>
        /// Creates a new REST element view model.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this view model operates on.</param>
        public ElementViewModel(IElementEndpoint<TEntity> endpoint) : base(endpoint)
        {
        }

        public TEntity Entity { get; private set; }

        protected override async void OnActivate()
        {
            base.OnActivate();

            Entity = await Endpoint.ReadAsync(CancellationToken);
            NotifyOfPropertyChange(() => Entity);
        }
    }
}