using System.Configuration;

namespace XProjectNamespaceX.BusinessLogic
{
    public class MyServiceConfiguration
    {
        public string MySetting { get; private set; }

        // NOTE: Internal to hide from dependency injection
        internal MyServiceConfiguration(string mySetting)
        {
            MySetting = mySetting;
        }

        public MyServiceConfiguration()
            : this(ConfigurationManager.AppSettings["MyServiceMySetting"])
        {
        }
    }
}