namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command operating on a <see cref="RestCollection{TEntity}"/> using <see cref="RestElement{TEntity}"/>s.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <see cref="RestCollection{TEntity}"/> represents.</typeparam>
    public class CollectionCommand<TEntity> : CollectionCommandBase<TEntity, RestCollection<TEntity>, RestElement<TEntity>>
    {
        /// <summary>
        /// Creates a new REST collection command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public CollectionCommand(RestCollection<TEntity> endpoint) : base(endpoint)
        {
        }
    }
}