using System.Configuration;

namespace XProjectNamespaceX.BusinessLogic
{
    public class MyServiceConfiguration
    {
        public string MySetting { get; private set; }

        // NOTE: Must be internal to prevent Unity from considering this constructor as a candidate for dependency injection
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