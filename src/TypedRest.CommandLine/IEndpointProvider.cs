using TypedRest.Endpoints;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Builds <see cref="IEndpoint"/> instances.
    /// </summary>
    /// <typeparam name="T">The type of endpoint created.</typeparam>
    public interface IEndpointProvider<out T>
        where T : IEndpoint
    {
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
