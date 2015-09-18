using System.Collections.Generic;
using System.Threading.Tasks;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command operating on an <see cref="IRestEndpoint"/>.
    /// </summary>
    public interface IEndpointCommand
    {
        /// <summary>
        /// Parses the <paramref name="args"/> and executes the result.
        /// </summary>
        Task ExecuteAsync(IReadOnlyList<string> args);
    }
}