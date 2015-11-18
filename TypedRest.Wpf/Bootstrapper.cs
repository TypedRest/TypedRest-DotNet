using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
            if (e.Exception is InvalidDataException
                || e.Exception is UnauthorizedAccessException
                || e.Exception is KeyNotFoundException
                || e.Exception is InvalidOperationException
                || e.Exception is IndexOutOfRangeException
                || e.Exception is HttpRequestException)
            {
                MessageBox.Show(e.Exception.Message, "REST error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                e.Handled = true;
            }
            else if (e.Exception is OperationCanceledException)
                e.Handled = true;

            base.OnUnhandledException(sender, e);
        }
    }
}