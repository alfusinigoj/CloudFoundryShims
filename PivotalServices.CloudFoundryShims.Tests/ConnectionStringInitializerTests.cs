using System;
using Xunit;
using NSubstitute;
using Microsoft.Extensions.Configuration;
using Steeltoe.Extensions.Configuration.CloudFoundry;

namespace PivotalServices.CloudFoundryShims.Tests
{
    public class ConnectionStringInitializerTests : IDisposable
    {
        private readonly IWebConfigWriter _webConfigWriter;
        private readonly IConfigurationRoot _config;

        const string VcapServices = @"
{
  'SqlServer': [
    {
      'credentials': {
        'uid': 'school_user',
        'uri': 'jdbc:sqlserver://10.0.0.100:1433;databaseName=school_prod',
        'db': 'school',
        'pw': 'secret'
      },
      'syslog_drain_url': null,
      'label': 'SqlServer',
      'provider': null,
      'plan': 'sharedVM',
      'name': 'schooldb',
      'tags': [
        'sqlserver'
      ]
    }
  ]
}
";

        public ConnectionStringInitializerTests()
        {
            Environment.SetEnvironmentVariable("VCAP_SERVICES", VcapServices);
            _webConfigWriter = Substitute.For<IWebConfigWriter>();
            _config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddCloudFoundry()
                .Build();
        }

        [Fact]
        public void CtorThrowsWhenConfigurationIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ConnectionStringInitializer(null));
        }

        [Fact]
        public void CopyBoundSqlInstancesToWebConfigThrowsWhenWebConfigWriterIsNull()
        {
            var csi = new ConnectionStringInitializer(_config);
            Assert.Throws<ArgumentNullException>(() => csi.CopyBoundSqlInstancesToWebConfig(null));
        }

        [Fact]
        public void CopyBoundSqlInstancesToWebConfigCopiesAnyBoundSqlServer()
        {
            var csi = new ConnectionStringInitializer(_config);
            csi.CopyBoundSqlInstancesToWebConfig(_webConfigWriter);
            _webConfigWriter.Received().SetConnectionString("schooldb",
                "Data Source=10.0.0.100,1433;Initial Catalog=school_prod;User Id=school_user;Password=secret;");
        }

        public void Dispose()
        {
            Environment.SetEnvironmentVariable("VCAP_SERVICES", "");
        }
    }
}
