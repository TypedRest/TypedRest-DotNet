namespace TypedRest.Http;

/// <summary>
/// Represents a subset of a set of elements.
/// </summary>
/// <param name="elements">The returned elements.</param>
/// <param name="range">The range the <paramref name="elements"/> come from.</param>
/// <typeparam name="TEntity">The type of element the response contains.</typeparam>
public sealed class PartialResponse<TEntity>(IReadOnlyList<TEntity> elements, ContentRangeHeaderValue? range)
{
    /// <summary>
    /// The returned elements.
    /// </summary>
    public IReadOnlyList<TEntity> Elements { get; } = elements;

    /// <summary>
    /// The range the <see cref="Elements"/> come from.
    /// </summary>
    public ContentRangeHeaderValue? Range { get; } = range;

    /// <summary>
    /// Indicates whether the response reaches the end of the elements available on the server.
    /// </summary>
    public bool EndReached
    {
        get
        {
            if (Range?.To == null)
            {
                // No range specified, must be complete response
                return true;
            }
            else if (!Range.Length.HasValue)
            {
                // No length specified, can't be the end
                return false;
            }
            else return Range.To.Value == Range.Length.Value - 1;
        }
    }
}
