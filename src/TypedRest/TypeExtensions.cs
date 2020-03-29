using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace TypedRest
{
    /// <summary>
    /// Provides extension methods for <see cref="Type"/>.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Returns a constructor for <typeparamref name="TResult"/> with two arguments as a function.
        /// </summary>
        /// <typeparam name="T1">The type of the first constructor argument.</typeparam>
        /// <typeparam name="T2">The type of the second constructor argument.</typeparam>
        /// <typeparam name="TResult">The type to construct.</typeparam>
        /// <returns>The function; <c>null</c> if no suitable constructor was found.</returns>
        public static Func<T1, T2, TResult>? GetConstructor<T1, T2, TResult>()
        {
            var type = typeof(TResult);

            var types = new[] {typeof(T1), typeof(T2)};
            var constructor = type.GetConstructor(types);
            if (constructor == null)
                return null;

            var parameters = types.Select(Expression.Parameter).ToList();
            var newExpression = Expression.New(constructor, parameters);
            return Expression.Lambda<Func<T1, T2, TResult>>(newExpression, parameters).Compile();
        }

        /// <summary>
        /// Gets a property defined on a <paramref name="type"/> that is annotated with a specific <typeparamref name="TAttribute"/>.
        /// </summary>
        /// <returns>The property; <c>null</c> if no such property was found.</returns>
        public static PropertyInfo? GetPropertyWith<TAttribute>(this Type type)
            where TAttribute : Attribute
            => type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                   .FirstOrDefault(x => x.GetCustomAttribute<TAttribute>(inherit: true) != null);

        /// <summary>
        /// Converts a <paramref name="method"/> into a function that takes the object instance as a parameter.
        /// </summary>
        /// <typeparam name="TSource">The type of the class defining the method.</typeparam>
        /// <typeparam name="TResult">The return type of the method.</typeparam>
        public static Func<TSource, TResult> ToFunc<TSource, TResult>(this MethodInfo method)
        {
            var instance = Expression.Parameter(typeof(TSource), "instance");
            var getExpression = Expression.TypeAs(Expression.Call(instance, method), typeof(TResult));
            return Expression.Lambda<Func<TSource, TResult>>(getExpression, instance).Compile();
        }
    }
}
