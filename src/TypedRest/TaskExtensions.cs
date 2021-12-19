using System.Runtime.CompilerServices;

namespace TypedRest;

/// <summary>
/// Provides extension methods for <see cref="Task"/> and <see cref="Task{TResult}"/>.
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    /// Convenience wrapper for <see cref="Task.ConfigureAwait"/><c>(false)</c>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConfiguredTaskAwaitable NoContext(this Task task)
        => task.ConfigureAwait(continueOnCapturedContext: false);

    /// <summary>
    /// Convenience wrapper for <see cref="Task{T}.ConfigureAwait"/><c>(false)</c>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConfiguredTaskAwaitable<TResult> NoContext<TResult>(this Task<TResult> task)
        => task.ConfigureAwait(continueOnCapturedContext: false);
}