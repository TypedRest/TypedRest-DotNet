using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Microsoft.Practices.Unity;
using NLog;
using Topshelf;

namespace XProjectNamespaceX.Service
{
    /// <summary>
    /// Entry point for Topshelf.
    /// </summary>
    public class MyService : ServiceControl
    {
        private readonly IUnityContainer _container = UnityConfig.InitContainer();
        private readonly ILogger _logger = LogManager.GetLogger("XProjectNamespaceX");

        /// <summary>
        /// Starts the internal scheduler and the REST API.
        /// </summary>
        public bool Start(HostControl hostControl)
        {
            AppDomain.CurrentDomain.UnhandledException += (_, e) => _logger.Fatal(e.ExceptionObject as Exception);
            TaskScheduler.UnobservedTaskException += (_, e) => _logger.Fatal(e.Exception);

            var assemblyInfo = Assembly.GetExecutingAssembly().GetName();
            _logger.Info("Starting {0} v{1}", assemblyInfo.Name, assemblyInfo.Version);

            StartWebApi();

            return true;
        }

        /// <summary>
        /// Stops the REST API and the internal scheduler.
        /// </summary>
        public bool Stop(HostControl hostControl)
        {
            _container.Dispose();

            return true;
        }

        /// <summary>
        /// Starts the REST API.
        /// </summary>
        private void StartWebApi()
        {
            var startOptions = new StartOptions();
            foreach (string url in ConfigurationManager.AppSettings["BaseAddress"].Split(','))
                startOptions.Urls.Add(url);

            try
            {
                _logger.Info("Starting Web API");
                _container.RegisterInstance(WebApp.Start(startOptions, new OwinStartup(_container).Configuration));
                _logger.Info("Web API started");
            }
            catch (Exception ex) when (ex.InnerException?.Message == "Access is denied")
            {
                _logger.Error($@"You either need to run XProjectNameX with administrative privileges or execute this command to enable listening on HTTP:
netsh http add urlacl url={startOptions.Urls.FirstOrDefault()} user={Environment.MachineName}\{Environment.UserName}");
                throw;
            }
        }
    }
}