namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command operating on a <see cref="IPagedCollectionEndpoint{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <see cref="IPagedCollectionEndpoint{TEntity}"/> represents.</typeparam>
    public class PagedCollectionCommand<TEntity> : PagedCollectionCommandBase<TEntity, IPagedCollectionEndpoint<TEntity>, IElementEndpoint<TEntity>>
    {
        /// <summary>
        /// Creates a new REST paged collection command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public PagedCollectionCommand(IPagedCollectionEndpoint<TEntity> endpoint) : base(endpoint)
        {
        }

        protected override IEndpointCommand GetElementCommand(IElementEndpoint<TEntity> elementEndpoint) => new ElementCommand<TEntity>(elementEndpoint);
    }
}