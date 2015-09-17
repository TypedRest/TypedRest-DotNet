namespace TypedRest.Commands
{
    /// <summary>
    /// Command operating on a <see cref="IRestSet{TEntity}"/> using <see cref="ItemCommand{TEntity}"/>s.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <see cref="IRestSet{TEntity}"/> represents.</typeparam>
    public class SetCommand<TEntity> : SetCommandBase<IRestSet<TEntity>, TEntity>
    {
        /// <summary>
        /// Creates a new REST set command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public SetCommand(IRestSet<TEntity> endpoint) : base(endpoint)
        {
        }
    }
}