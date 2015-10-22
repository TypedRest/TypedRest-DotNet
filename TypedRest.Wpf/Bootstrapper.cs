using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using Caliburn.Micro;

namespace TypedRest.Wpf
{
    public class Bootstrapper<T> : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return base.SelectAssemblies().Concat(new[] {Assembly.GetExecutingAssembly()});
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<T>();
        }
    }
}