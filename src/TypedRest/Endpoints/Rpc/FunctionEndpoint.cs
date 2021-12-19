namespace TypedRest.Endpoints.Rpc
{
    /// <summary>
    /// RPC endpoint that takes <typeparamref name="TEntity"/> as input and returns <typeparamref name="TResult"/> as output when invoked.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint takes as input.</typeparam>
    /// <typeparam name="TResult">The type of entity the endpoint returns as output.</typeparam>
    public class FunctionEndpoint<TEntity, TResult> : RpcEndpointBase, IFunctionEndpoint<TEntity, TResult>
    {
        /// <summary>
        /// Creates a new function endpoint with a relative URI.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s.</param>
        public FunctionEndpoint(IEndpoint referrer, Uri relativeUri)
            : base(referrer, relativeUri)
        {}

        /// <summary>
        /// Creates a new function endpoint with a relative URI.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Add a <c>./</c> prefix here to imply a trailing slash <paramref name="referrer"/>'s URI.</param>
        public FunctionEndpoint(IEndpoint referrer, string relativeUri)
            : base(referrer, relativeUri)
        {}

        public async Task<TResult> InvokeAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var response = await HandleAsync(() => HttpClient.PostAsync(Uri, entity, Serializer, cancellationToken)).NoContext();

            return await response.Content.ReadAsAsync<TResult>(Serializer, cancellationToken).NoContext();
        }
    }
}
