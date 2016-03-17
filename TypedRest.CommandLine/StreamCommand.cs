namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command operating on a <see cref="IStreamEndpoint{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <see cref="StreamEndpoint{TEntity}"/> represents.</typeparam>
    public class StreamCommand<TEntity> : StreamCommandBase<TEntity, IStreamEndpoint<TEntity>, IElementEndpoint<TEntity>>
    {
        /// <summary>
        /// Creates a new REST stream command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public StreamCommand(IStreamEndpoint<TEntity> endpoint) : base(endpoint)
        {
        }

        protected override IEndpointCommand GetElementCommand(IElementEndpoint<TEntity> elementEndpoint)
        {
            return new ElementCommand<TEntity>(elementEndpoint);
        }
    }
}