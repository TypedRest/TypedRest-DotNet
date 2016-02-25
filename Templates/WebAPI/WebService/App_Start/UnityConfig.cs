using Microsoft.Practices.Unity;

namespace XProjectNamespaceX.WebService
{
    public static class UnityConfig
    {
        public static IUnityContainer InitContainer()
        {
            return new UnityContainer()
                .RegisterByConvention();
        }

        private static IUnityContainer RegisterByConvention(this IUnityContainer container)
        {
            return container.RegisterTypes(AllClasses.FromLoadedAssemblies(), WithMappings.FromMatchingInterface,
                    getLifetimeManager: _ => ScopeSession);
        }

        private static LifetimeManager ScopeSession { get { return new HierarchicalLifetimeManager(); } }
    }
}
