using System;
using Microsoft.Extensions.Configuration;

namespace PivotalServices.CloudFoundryShims
{
    public static class Startup
    {
        private static bool Initialized;
        private static readonly object Lock = new object();

        public static void Initialize()
        {
            lock (Lock)
            {
                if (!Initialized)
                {
                    DoInitialization();
                    Initialized = true;
                }
            }
        }

        private static void DoInitialization()
        {
            var config = ServerConfig.GetConfiguration();
            InitializeWebConfig(config);
            InitializeDefaultErrorHandler();
        }

        private static void InitializeWebConfig(IConfigurationRoot config)
        {
            using (var webConfig = new WebConfigFileReaderWriter())
            {
                InitializeConnectionStrings(webConfig, config);
                InitializeAppSettings(webConfig, config);
                InitializeCustomErrorsSettings(webConfig, config);
            }
        }

        private static void InitializeConnectionStrings(WebConfigFileReaderWriter webConfig, IConfigurationRoot config)
        {
            var connectionStringInitializer = new ConnectionStringInitializer(config);
            connectionStringInitializer.CopyBoundSqlInstancesToWebConfig(webConfig);
        }

        private static void InitializeAppSettings(WebConfigFileReaderWriter webConfig, IConfigurationRoot config)
        {
            var appSettingsInitializer = new AppSettingsInitializer(config, webConfig);
            appSettingsInitializer.CopyConfigToAppSettings(webConfig);
        }

        private static void InitializeCustomErrorsSettings(WebConfigFileReaderWriter webConfig, IConfigurationRoot config)
        {
            var customErrorsInitializer = new CustomErrorsInitializer(config);
            customErrorsInitializer.SetCustomErrorsMode(webConfig);
        }

        private static void InitializeDefaultErrorHandler()
        {
            DefaultApplicationErrorHandler.Register();
        }
    }
}
