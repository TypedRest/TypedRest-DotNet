namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command operating on a <see cref="IStreamingCollectionEndpoint{TEntity,TElementEndpoint}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    /// <typeparam name="TElementEndpoint">The specific type of <see cref="IElementEndpoint{TEntity}"/> the endpoint provides for individual <typeparamref name="TEntity"/>s.</typeparam>
    /// <typeparam name="TElementCommand">The specific type of <see cref="IEndpointCommand"/> is used to handle <typeparamref name="TElementEndpoint"/>s. This must be a non-abstract class with a constructor that takes a <typeparamref name="TElementEndpoint"/>, unless you override <c>BuildElementCommand</c>.</typeparam>
    public class StreamingCollectionCommand<TEntity, TElementEndpoint, TElementCommand> : StreamingCollectionCommand<TEntity, IStreamingCollectionEndpoint<TEntity, TElementEndpoint>, TElementEndpoint, TElementCommand>
        where TElementEndpoint : class, IEndpoint
        where TElementCommand : class, IEndpointCommand
    {
        /// <summary>
        /// Creates a new REST streaming collection command.
        /// </summary>
        /// <param name="endpoint">The endpoint this command operates on.</param>
        public StreamingCollectionCommand(IStreamingCollectionEndpoint<TEntity, TElementEndpoint> endpoint)
            : base(endpoint)
        {}
    }

    /// <summary>
    /// Command operating on a <see cref="IStreamingCollectionEndpoint{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    public class StreamingCollectionCommand<TEntity> : StreamingCollectionCommand<TEntity, IElementEndpoint<TEntity>, ElementCommand<TEntity>>
    {
        /// <summary>
        /// Creates a new REST streaming collection command.
        /// </summary>
        /// <param name="endpoint">The endpoint this command operates on.</param>
        public StreamingCollectionCommand(IStreamingCollectionEndpoint<TEntity> endpoint)
            : base(endpoint)
        {}

        protected override ElementCommand<TEntity> BuildElementCommand(IElementEndpoint<TEntity> elementEndpoint) => new ElementCommand<TEntity>(elementEndpoint);
    }
}
