using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// Provides extension methods for <seealso cref="Task"/> and <seealso cref="Task{TResult}"/>.
    /// </summary>
    internal static class TaskExtensions
    {
        /// <summary>
        /// Convenience wrapper for <seealso cref="Task.ConfigureAwait"/><c>(false)</c>.
        /// </summary>
        public static ConfiguredTaskAwaitable NoContext(this Task task)
            => task.ConfigureAwait(continueOnCapturedContext: false);

        /// <summary>
        /// Convenience wrapper for <seealso cref="Task{T}.ConfigureAwait"/><c>(false)</c>.
        /// </summary>
        public static ConfiguredTaskAwaitable<TResult> NoContext<TResult>(this Task<TResult> task)
            => task.ConfigureAwait(continueOnCapturedContext: false);
    }
}