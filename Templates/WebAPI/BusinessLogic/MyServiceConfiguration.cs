using System.Configuration;

namespace XProjectNamespaceX.BusinessLogic
{
    /// <summary>
    /// Configuration for <see cref="MyService"/>.
    /// </summary>
    public class MyServiceConfiguration
    {
        /// <summary>
        /// A setting.
        /// </summary>
        public string MySetting { get; private set; }

        /// <summary>
        /// Creates a custom configuration.
        /// </summary>
        /// <param name="mySetting">A setting.</param>
        public MyServiceConfiguration(string mySetting)
        {
            MySetting = mySetting;
        }

        /// <summary>
        /// Loads configuration from <see cref="ConfigurationManager.AppSettings"/>.
        /// </summary>
        public static MyServiceConfiguration FromAppSettings()
        {
            return new MyServiceConfiguration(
                mySetting: ConfigurationManager.AppSettings["MyServiceMySetting"]);
        }
    }
}