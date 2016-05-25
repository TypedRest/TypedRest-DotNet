using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TypedRest
{
    internal class PropertyHelper
    {
        private static readonly ConcurrentDictionary<Type, PropertyHelper[]> _reflectionCache =
            new ConcurrentDictionary<Type, PropertyHelper[]>();

        private static readonly MethodInfo _callPropertyGetterOpenGenericMethod =
            typeof(PropertyHelper).GetMethod("CallPropertyGetter", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly MethodInfo _callPropertyGetterByReferenceOpenGenericMethod =
            typeof(PropertyHelper).GetMethod("CallPropertyGetterByReference",
                                             BindingFlags.Static | BindingFlags.NonPublic);

        private delegate TValue ByRefFunc<TDeclaringType, out TValue>(ref TDeclaringType arg);

        private readonly Func<object, object> _valueGetter;

        public string Name { get; }

        private PropertyHelper(PropertyInfo property)
        {
            Name = property.Name;
            _valueGetter = MakeFastPropertyGetter(property);
        }

        public string GetValueAsString(object instance)
        {
            object value = GetValue(instance);

            return value?.ToString();
        }

        public static PropertyHelper[] GetProperties(object instance)
        {
            return GetProperties(instance, CreateInstance, _reflectionCache);
        }

        private object GetValue(object instance)
        {
            return _valueGetter(instance);
        }

        private static Func<object, object> MakeFastPropertyGetter(PropertyInfo propertyInfo)
        {
            MethodInfo getMethod = propertyInfo.GetGetMethod();
            Type reflectedType = getMethod.ReflectedType;
            Type returnType = getMethod.ReturnType;
            Delegate @delegate;

            if (reflectedType.IsValueType)
            {
                @delegate = Delegate
                    .CreateDelegate(type: typeof(Func<object, object>),
                                    firstArgument:
                                        getMethod.CreateDelegate(typeof(ByRefFunc<,>).MakeGenericType(reflectedType,
                                                                                                      returnType)),
                                    method:
                                        _callPropertyGetterByReferenceOpenGenericMethod.MakeGenericMethod(
                                            reflectedType, returnType));
            }
            else
            {
                @delegate = Delegate
                    .CreateDelegate(type: typeof(Func<object, object>),
                                    firstArgument:
                                        getMethod.CreateDelegate(typeof(Func<,>).MakeGenericType(reflectedType,
                                                                                                 returnType)),
                                    method:
                                        _callPropertyGetterOpenGenericMethod.MakeGenericMethod(reflectedType, returnType));
            }

            return (Func<object, object>)@delegate;
        }

        private static PropertyHelper CreateInstance(PropertyInfo property)
        {
            return new PropertyHelper(property);
        }

        /// <summary>
        /// Used implicitly.
        /// </summary>
        private static object CallPropertyGetter<TDeclaringType, TValue>(Func<TDeclaringType, TValue> getter,
            object @this)
        {
            return getter((TDeclaringType)@this);
        }

        /// <summary>
        /// Used implicitly.
        /// </summary>
        private static object CallPropertyGetterByReference<TDeclaringType, TValue>(
            ByRefFunc<TDeclaringType, TValue> getter, object @this)
        {
            TDeclaringType declaringType = (TDeclaringType)@this;
            return getter(ref declaringType);
        }

        /// <summary>
        /// Used implicitly.
        /// </summary>
        private static void CallPropertySetter<TDeclaringType, TValue>(Action<TDeclaringType, TValue> setter,
            object @this, object value)
        {
            setter((TDeclaringType)@this, (TValue)value);
        }

        private static PropertyHelper[] GetProperties(object instance,
            Func<PropertyInfo, PropertyHelper> createPropertyHelper, ConcurrentDictionary<Type, PropertyHelper[]> cache)
        {
            Type type = instance.GetType();
            PropertyHelper[] array;
            if (!cache.TryGetValue(type, out array))
            {
                IEnumerable<PropertyInfo> propertyInfos =
                    type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .Where(prop =>
                        {
                            if (prop.GetIndexParameters().Length == 0)
                                return prop.GetMethod != null;

                            return false;
                        });

                List<PropertyHelper> propertyHelperList = new List<PropertyHelper>();
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    PropertyHelper propertyHelper = createPropertyHelper(propertyInfo);
                    propertyHelperList.Add(propertyHelper);
                }

                array = propertyHelperList.ToArray();
                cache.TryAdd(type, array);
            }

            return array;
        }
    }
}