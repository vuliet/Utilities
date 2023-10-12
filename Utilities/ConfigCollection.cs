using Microsoft.Extensions.Configuration;

namespace Utilities
{
    public class ConfigCollection
    {
        private readonly IConfigurationRoot configuration;

        public static ConfigCollection Instance { get; } = new ConfigCollection();

        protected ConfigCollection()
        {
            var env = EnviromentUtils.GetEnv();

            configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"appsettings.{env}.json", optional: true)
               .AddEnvironmentVariables()
               .Build();
        }

        public IConfigurationRoot GetConfiguration()
        {
            return configuration;
        }
    }
}
