using System;
using Microsoft.Extensions.Configuration;
using Steeltoe.CloudFoundry.Connector;
using Steeltoe.CloudFoundry.Connector.Services;
using Steeltoe.CloudFoundry.Connector.SqlServer;
using System.Data.SqlClient;

namespace PivotalServices.CloudFoundryShims
{
    /// <summary>
    /// Initializes the web.config connectionString section from CF bound services
    /// </summary>
    public class ConnectionStringInitializer
    {
        private readonly IConfigurationRoot _config;

        public ConnectionStringInitializer(IConfigurationRoot config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config), "The application config is required");
        }

        public void CopyBoundSqlInstancesToWebConfig(IWebConfigWriter webConfig)
        {
            if (webConfig == null)
            {
                throw new ArgumentNullException(nameof(webConfig), "The web config writer is required");
            }

            SqlServerProviderConnectorOptions sqlConnectorOptions = new SqlServerProviderConnectorOptions(_config);
            foreach (SqlServerServiceInfo sqlServiceInfo in _config.GetServiceInfos<SqlServerServiceInfo>())
            {
                CopyBoundSqlConnection(webConfig, sqlServiceInfo, sqlConnectorOptions);
            }
        }

        private static void CopyBoundSqlConnection(
            IWebConfigWriter webConfig,
            SqlServerServiceInfo sqlServiceInfo,
            SqlServerProviderConnectorOptions sqlConnectorOptions)
        {
            SqlServerProviderConnectorFactory sqlConnectorFactory = new SqlServerProviderConnectorFactory(
                sqlServiceInfo, sqlConnectorOptions, typeof(SqlConnection));
            webConfig.SetConnectionString(sqlServiceInfo.Id, sqlConnectorFactory.CreateConnectionString());
        }
    }
}
