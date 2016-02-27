namespace XProjectNamespaceX.BusinessLogic
{
    public class MyService : IMyService
    {
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