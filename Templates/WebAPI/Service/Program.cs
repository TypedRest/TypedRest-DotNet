using System.Diagnostics;
using Topshelf;

namespace XProjectNamespaceX.Service
{
    public static class Program
    {
        public static void Main()
        {
            HostFactory.Run(config =>
            {
                config
                    .Service<MyService>()
                    .StartAutomatically()
                    .RunAsLocalService()
                    .BeforeInstall(settings =>
                    {
                        // Prepare Event Log at install-time since we have reduced privileges at run-time
                        if (!EventLog.SourceExists(settings.ServiceName))
                            EventLog.CreateEventSource(settings.ServiceName, "Application");
                    })
                    .UseAssemblyInfoForServiceInfo();
                config.UseNLog();
            });
        }
    }
}