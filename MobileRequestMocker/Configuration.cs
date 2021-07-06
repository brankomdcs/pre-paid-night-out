using System.Configuration;

namespace MobileRequestMocker
{
    public class Configuration
    {
        private static Configuration instance = null;

        private Configuration()
        {
            var proxyUrl = ConfigurationManager.AppSettings.Get("ReverseProxyUrl");
            ReverseProxyUrl = proxyUrl ?? "http://localhost:19081";
        }

        public static Configuration GetInstance() {
            if (instance == null) {
                instance = new Configuration();
            }
            return instance;
        }
        public string ReverseProxyUrl { get; }
    }
}
