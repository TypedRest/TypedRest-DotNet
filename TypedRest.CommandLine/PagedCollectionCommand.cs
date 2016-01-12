namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command operating on a <see cref="PagedCollectionEndpoint{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <see cref="PagedCollectionEndpoint{TEntity}"/> represents.</typeparam>
    public class PagedCollectionCommand<TEntity> : PagedCollectionCommandBase<TEntity, PagedCollectionEndpoint<TEntity>, ElementEndpoint<TEntity>>
    {
        /// <summary>
        /// Creates a new REST paged collection command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public PagedCollectionCommand(PagedCollectionEndpoint<TEntity> endpoint) : base(endpoint)
        {
        }

        protected override IEndpointCommand GetElementCommand(ElementEndpoint<TEntity> elementEndpoint)
        {
            return new ElementCommand<TEntity>(elementEndpoint);
        }
    }
}