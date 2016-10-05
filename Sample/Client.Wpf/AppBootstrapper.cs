using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using TypedRest;
using TypedRestSample.Client.Wpf.ViewModels;

namespace TypedRestSample.Client.Wpf
{
    public class AppBootstrapper : BootstrapperBase
    {
        public AppBootstrapper()
        {
            Initialize();
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return base.SelectAssemblies().Concat(new[] { Assembly.GetExecutingAssembly() });
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var ex = e.Exception;
            Trace.TraceError(ex.ToString());
            MessageBox.Show(ex.GetFullMessage(), "Unexpected error", MessageBoxButton.OK, MessageBoxImage.Stop);

            e.Handled = true;
            base.OnUnhandledException(sender, e);
        }
    }
}