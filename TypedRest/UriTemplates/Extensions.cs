// Resta.UriTemplates
// Copyright(c) 2013 Pavel Shkarin
// Source: https://github.com/a7b0/uri-templates
// License: The MIT License(MIT)

using System.Collections;
using System.Collections.Generic;

namespace TypedRest.UriTemplates
{
    internal static class Extensions
    {
        public static int Count<T>(this IEnumerable<T> source)
        {
            var typedCollection = source as ICollection<string>;

            if (typedCollection != null)
            {
                return typedCollection.Count;
            }

            var collection = source as ICollection;

            if (collection != null)
            {
                return collection.Count;
            }

            var size = 0;

            foreach (var item in source)
            {
                size++;
            }

            return size;
        }
    }
}