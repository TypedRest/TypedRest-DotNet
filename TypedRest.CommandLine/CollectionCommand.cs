namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command operating on a <see cref="CollectionEndpoint{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <see cref="CollectionEndpoint{TEntity}"/> represents.</typeparam>
    public class CollectionCommand<TEntity> : CollectionCommandBase<TEntity, CollectionEndpoint<TEntity>, ElementEndpoint<TEntity>>
    {
        /// <summary>
        /// Creates a new REST collection command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public CollectionCommand(CollectionEndpoint<TEntity> endpoint) : base(endpoint)
        {
        }

        protected override IEndpointCommand GetElementCommand(ElementEndpoint<TEntity> elementEndpoint)
        {
            return new ElementCommand<TEntity>(elementEndpoint);
        }
    }
}