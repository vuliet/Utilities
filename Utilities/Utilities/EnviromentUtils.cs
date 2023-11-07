using Microsoft.Extensions.Configuration;

namespace Utilities.Utilities
{
    public static class EnviromentUtils
    {
        public static string GetConfig(string code)
        {
            IConfigurationRoot configuration = ConfigCollection.Instance.GetConfiguration();
            var value = configuration[code];
            return value ?? string.Empty;
        }

        public static string GetConfig(IConfiguration configuration, string code)
        {
            var value = configuration[code];
            return value ?? string.Empty;
        }

        public static string GetEnv()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        }

        public static bool IsDevelopment() => GetEnv().Equals("Development");
        public static bool IsLocal() => GetEnv().Equals("Local");
        public static bool IsProduction() => GetEnv().Equals("Production");
    }
}
