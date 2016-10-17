namespace TypedRest
{
    /// <summary>
    /// Builds <see cref="IEndpoint"/> instances.
    /// </summary>
    /// <typeparam name="T">The type of endpoint created.</typeparam>
    public interface IEndpointProvider<out T>
        where T : class, IEndpoint
    {
        /// <summary>
        /// Resets any stored endpoint URI information.
        /// </summary>
        void ResetUri();

        /// <summary>
        /// Clears any cached authentication information.
        /// </summary>
        void ResetAuthentication();

        /// <summary>
        /// Builds a new endpoint.
        /// </summary>
        T Build();
    }
}