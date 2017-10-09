using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// Provides extension methods for <seealso cref="IDictionary{TKey,TValue}"/>s.
    /// </summary>
    internal static class DictionaryExtensions
    {
        /// <summary>
        /// Returns the element with the specified <paramref name="key"/> from the <paramref name="dictionary"/>.
        /// Creates, adds and returns a new value using <paramref name="valueFactory"/> if no match was found.
        /// </summary>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
            Func<TValue> valueFactory)
        {
            if (!dictionary.TryGetValue(key, out var value))
                dictionary.Add(key, value = valueFactory());
            return value;
        }

        /// <summary>
        /// Returns the element with the specified <paramref name="key"/> from the <paramref name="dictionary"/>.
        /// Creates, adds and returns a new <typeparamref name="TValue"/> instance if no match was found.
        /// </summary>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
            where TValue : new()
            => dictionary.GetOrAdd(key, () => new TValue());

        /// <summary>
        /// Returns the element with the specified <paramref name="key"/> from the <paramref name="dictionary"/>.
        /// Creates, adds and returns a new value using <paramref name="valueFactory"/> if no match was found.
        /// </summary>
        public static async Task<TValue> GetOrAddAsync<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<Task<TValue>> valueFactory)
        {
            if (!dictionary.TryGetValue(key, out var value))
                dictionary.Add(key, value = await valueFactory());
            return value;
        }
    }
}