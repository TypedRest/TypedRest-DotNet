namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command operating on a <see cref="StreamEndpoint{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <see cref="StreamEndpoint{TEntity}"/> represents.</typeparam>
    public class StreamCommand<TEntity> : StreamCommandBase<TEntity, StreamEndpoint<TEntity>, ElementEndpoint<TEntity>>
    {
        /// <summary>
        /// Creates a new REST stream command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public StreamCommand(StreamEndpoint<TEntity> endpoint) : base(endpoint)
        {
        }

        protected override ICommand GetElementCommand(ElementEndpoint<TEntity> element)
        {
            return new ElementCommand<TEntity>(element);
        }
    }
}