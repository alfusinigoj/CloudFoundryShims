using Microsoft.Extensions.Configuration;
using Steeltoe.Extensions.Configuration.CloudFoundry;

namespace PivotalServices.CloudFoundryShims
{
    public static class ServerConfig
    {
        public static IConfigurationRoot GetConfiguration()
        {
            return BuildConfiguration();
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .AddCloudFoundry()
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
