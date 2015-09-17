namespace TypedRest.Commands
{
    /// <summary>
    /// Command operating on a <see cref="RestSet{TEntity}"/> using <see cref="RestElement{TEntity}"/>s.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <see cref="RestSet{TEntity}"/> represents.</typeparam>
    public class SetCommand<TEntity> : SetCommandBase<TEntity, RestElement<TEntity>, RestSet<TEntity>>
    {
        /// <summary>
        /// Creates a new REST set command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public SetCommand(RestSet<TEntity> endpoint) : base(endpoint)
        {
        }
    }
}