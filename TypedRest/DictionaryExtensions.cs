using System.Collections.Generic;

namespace TypedRest
{
    /// <summary>
    /// Provides extension methods for <seealso cref="IDictionary{TKey,TValue}"/>s.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Returns the element with the specified <paramref name="key"/> from the <paramref name="dictionary"/>.
        /// Creates, adds and returns a new <typeparamref name="TValue"/> if no match was found.
        /// </summary>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
            where TValue : new()
        {
            TValue value;
            if (!dictionary.TryGetValue(key, out value))
                dictionary.Add(key, value = new TValue());
            return value;
        }
    }
}