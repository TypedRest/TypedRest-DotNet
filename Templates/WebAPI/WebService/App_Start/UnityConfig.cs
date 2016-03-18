using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using Microsoft.Practices.Unity;

namespace XProjectNamespaceX.WebService
{
    public static class UnityConfig
    {
        public static IUnityContainer InitContainer()
        {
            return new UnityContainer()
                .RegisterByConvention()
                .RegisterSingletons();
        }

        /// <summary>
        /// Automatically match classes and interfaces with matching names (e.g. IMyService and MyService).
        /// </summary>
        private static IUnityContainer RegisterByConvention(this IUnityContainer container)
        {
            return container.RegisterTypes(FromAssembliesInSearchPath(), WithMappings.FromMatchingInterface, WithName.Default, _ => Scope.Session);
        }

        private static IUnityContainer RegisterSingletons(this IUnityContainer container)
        {
            return container
                //.RegisterType<MySingleton>(Scope.Singleton)
                ;
        }

        private static class Scope
        {
            /// <summary>Single instance per process.</summary>
            public static LifetimeManager Singleton { get { return new ContainerControlledLifetimeManager(); } }

            /// <summary>One instance per HTTP request.</summary>
            public static LifetimeManager Session { get { return new HierarchicalLifetimeManager(); } }
        }

        #region Registration by convention
        private static IEnumerable<Type> FromAssembliesInSearchPath()
        {
            return GetTypes(GetAssembliesInSearchPath());
        }

        private static IEnumerable<Assembly> GetAssembliesInSearchPath()
        {
            try
            {
                string searchPath = (AppDomain.CurrentDomain.RelativeSearchPath == null)
                    ? AppDomain.CurrentDomain.BaseDirectory
                    : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.RelativeSearchPath);

                return Directory.EnumerateFiles(searchPath, "*.dll")
                    .Select(x => LoadAssembly(Path.GetFileNameWithoutExtension(x)))
                    .Where(x => x != null);
            }
            catch (SecurityException)
            {
                return new Assembly[0];
            }
        }

        private static Assembly LoadAssembly(string assemblyName)
        {
            try
            {
                return Assembly.Load(assemblyName);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (FileLoadException)
            {
                return null;
            }
            catch (BadImageFormatException)
            {
                return null;
            }
        }

        private static IEnumerable<Type> GetTypes(IEnumerable<Assembly> assemblies)
        {
            return assemblies.SelectMany(assembly =>
            {
                try
                {
                    return GetTypes(assembly.DefinedTypes);
                }
                catch (ReflectionTypeLoadException ex)
                {
                    return GetTypes(ex.Types
                        .TakeWhile(x => x != null)
                        .Select(x => x.GetTypeInfo()));
                }
            });
        }

        private static IEnumerable<Type> GetTypes(IEnumerable<TypeInfo> typeInfos)
        {
            return typeInfos
                .Where(x => x.IsClass & !x.IsAbstract && !x.IsValueType && x.IsVisible)
                .Select(ti => ti.AsType());
        }
        #endregion
    }
}
