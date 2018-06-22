using System;
using System.Collections.Generic;
using Xunit;
using NSubstitute;
using Microsoft.Extensions.Configuration;

namespace PivotalServices.CloudFoundryShims.Tests
{
    public class AppSettingsInitializerTests
    {
        private readonly IWebConfigReader _webConfigReader;
        private readonly IWebConfigWriter _webConfigWriter;
        private readonly IConfigurationRoot _config;

        public AppSettingsInitializerTests()
        {
            _webConfigWriter = Substitute.For<IWebConfigWriter>();
            _webConfigReader = Substitute.For<IWebConfigReader>();

            var existingAppSettings = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("app_setting_1", "old_value1"),
                new KeyValuePair<string, string>("app_setting_2", "old_value2")
            };
            _webConfigReader.GetAppSettings().Returns(existingAppSettings.AsReadOnly());

            _config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                { "app_setting_2", "new_value_2" },
                { "app_setting_3", "new_value_3" }
            }).Build();
        }

        [Fact]
        public void CtorThrowsWhenConfigurationIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new AppSettingsInitializer(null, _webConfigReader));
        }

        [Fact]
        public void CtorThrowsWhenWebConfigReaderIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new AppSettingsInitializer(_config, null));
        }

        [Fact]
        public void CopyConfigToAppSettingsThrowsWhenWebConfigWriterIsNull()
        {
            var asi = new AppSettingsInitializer(_config, _webConfigReader);
            Assert.Throws<ArgumentNullException>(() => asi.CopyConfigToAppSettings(null));
        }

        [Fact]
        public void CopyConfigToAppSettingsDoesNotUpdateValuesForKeysThatDoNotAlreadyExist()
        {
            var asi = new AppSettingsInitializer(_config, _webConfigReader);
            asi.CopyConfigToAppSettings(_webConfigWriter);
            _webConfigWriter.DidNotReceive().SetAppSetting("app_setting_3", "new_value_3");
        }

        [Fact]
        public void CopyConfigToAppSettingsUpdatesValuesFromConfig()
        {
            var asi = new AppSettingsInitializer(_config, _webConfigReader);
            asi.CopyConfigToAppSettings(_webConfigWriter);
            _webConfigWriter.Received().SetAppSetting("app_setting_2", "new_value_2");
        }
    }
}
