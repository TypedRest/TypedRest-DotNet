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
            return container.RegisterTypes(AllClasses.FromAssembliesInBasePath(), WithMappings.FromMatchingInterface, WithName.Default, _ => Scope.Session);
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
    }
}
