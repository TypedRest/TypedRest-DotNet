using System;
using System.Linq;

namespace TypedRest
{
    /// <summary>
    /// Provides extension methods for arrays.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Searches for the starting index of a specific pattern/sequence in the array.
        /// </summary>
        /// <param name="array">The array to search.</param>
        /// <param name="pattern">The pattern to search for.</param>
        /// <param name="startIndex">The starting index of the search.</param>
        /// <param name="count">The maximum number of elements in the array to search.</param>
        /// <typeparam name="T">The type of the elements of the array.</typeparam>
        /// <returns>The start index of the first occurrence of the pattern, if found; otherwise, –1.</returns>
        public static int IndexOfPattern<T>(this T[] array, T[] pattern, int startIndex = 0, int count = int.MaxValue)
            where T : IEquatable<T>
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));
            if (pattern.Length == 0) throw new ArgumentException("Pattern must not be empty.", nameof(pattern));
            if (startIndex < 0) throw new ArgumentException("Start index must not be negative.", nameof(startIndex));
            if (count > array.Length - startIndex) throw new ArgumentException("Count must not exceed end of array.", nameof(count));
            if (count < 0) count = array.Length - startIndex;

            for (int arrayIndex = startIndex; arrayIndex < startIndex + count - pattern.Length; arrayIndex++)
            {
                if (!pattern.Where((patternPart, patternIndex) => !Equals(patternPart, array[arrayIndex + patternIndex])).Any())
                    return arrayIndex;
            }
            return -1;
        }
    }
}
