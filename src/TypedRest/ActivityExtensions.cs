using System.Diagnostics;

namespace TypedRest;

/// <summary>
/// Provides extension methods for <see cref="Activity"/>.
/// </summary>
public static class ActivityExtensions
{
    /// <summary>
    /// Updates the <paramref name="activity"/> to have tags encoding the specified <paramref name="exception"/>.
    /// </summary>
    public static Activity AddException(this Activity activity, Exception exception)
        => activity.AddTag("error", "true")
                   .AddTag("error.type", exception.GetType().Name)
                   .AddTag("error.message", exception.Message);
}