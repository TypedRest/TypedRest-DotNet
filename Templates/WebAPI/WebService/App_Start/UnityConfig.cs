using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using Microsoft.Practices.Unity;
using Unity.Patterns;
using XProjectNamespaceX.BusinessLogic;

namespace XProjectNamespaceX.WebService
{
    /// <summary>
    /// Configuration of Unity dependency injection.
    /// </summary>
    public static class UnityConfig
    {
        /// <summary>
        /// Builds a Unity dependency injection container.
        /// </summary>
        public static IUnityContainer InitContainer()
        {
            return new UnityContainer()
                .RegisterByConvention()
                .RegisterInstance(MyServiceConfiguration.FromAppSettings());
        }
    }
}
