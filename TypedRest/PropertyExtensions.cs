using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TypedRest
{
    /// <summary>
    /// Provides extension methods for reflection on properties.
    /// </summary>
    internal static class PropertyExtensions
    {
        /// <summary>
        /// Returns all properties with public getters defined on a type.
        /// </summary>
        public static IEnumerable<PropertyInfo> GetPublicProperties(this Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.GetGetMethod() != null);
        }

        /// <summary>
        /// Determines whether a property is marked with <typeparamref name="TAttribute"/>.
        /// </summary>
        public static bool HasAttribute<TAttribute>(this PropertyInfo prop)
            where TAttribute : Attribute
        {
            return prop.GetCustomAttributes(typeof(TAttribute), inherit: true).Any();
        }
    }
}