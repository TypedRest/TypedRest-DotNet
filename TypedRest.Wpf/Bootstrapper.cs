using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
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

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Trace.TraceError(e.Exception.ToString());
            MessageBox.Show(e.Exception.Message, "Unexpected error", MessageBoxButton.OK, MessageBoxImage.Stop);
            e.Handled = true;

            base.OnUnhandledException(sender, e);
        }
    }
}