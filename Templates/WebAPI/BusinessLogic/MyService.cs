using NLog;

namespace XProjectNamespaceX.BusinessLogic
{
    public class MyService : IMyService
    {
        private readonly ILogger _logger = LogManager.GetLogger("BusinessLogic");

        private readonly MyServiceConfiguration _configuration;

        public MyService(MyServiceConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Action()
        {
        }
    }
}