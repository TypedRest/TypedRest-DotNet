namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command operating on a <see cref="ICollectionEndpoint{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <see cref="ICollectionEndpoint{TEntity}"/> represents.</typeparam>
    public class CollectionCommand<TEntity> : CollectionCommandBase<TEntity, ICollectionEndpoint<TEntity>, IElementEndpoint<TEntity>>
    {
        /// <summary>
        /// Creates a new REST collection command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public CollectionCommand(ICollectionEndpoint<TEntity> endpoint) : base(endpoint)
        {
        }

        protected override IEndpointCommand GetElementCommand(IElementEndpoint<TEntity> elementEndpoint)
        {
            return new ElementCommand<TEntity>(elementEndpoint);
        }
    }
}