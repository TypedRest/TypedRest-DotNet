using System.Collections.Generic;
using System.Net.Http.Headers;

namespace TypedRest.Http
{
    /// <summary>
    /// Represents a subset of a set of elements.
    /// </summary>
    /// <typeparam name="TEntity">The type of element the response contains.</typeparam>
    public class PartialResponse<TEntity>
    {
        /// <summary>
        /// The returned elements.
        /// </summary>
        public readonly IReadOnlyList<TEntity> Elements;

        /// <summary>
        /// The range the <see cref="Elements"/> come from.
        /// </summary>
        public readonly ContentRangeHeaderValue Range;

        /// <summary>
        /// Creates a new partial response.
        /// </summary>
        /// <param name="elements">The returned elements.</param>
        /// <param name="range">The range the <paramref name="elements"/> come from.</param>
        public PartialResponse(IReadOnlyList<TEntity> elements, ContentRangeHeaderValue range)
        {
            Elements = elements;
            Range = range;
        }

        /// <summary>
        /// Indicates whether the response reaches the end of the elements available on the server.
        /// </summary>
        public bool EndReached
        {
            get
            {
                if (!Range.HasRange)
                {
                    // No range specified, must be complete response
                    return true;
                }

                if (!Range.HasLength)
                {
                    // No lenth specified, can't be end
                    return false;
                }

                return (Range.To.Value == Range.Length.Value - 1);
            }
        }
    }
}
