using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Web.Configuration;

namespace PivotalServices.CloudFoundryShims
{
    internal class WebConfigFileReaderWriter : IWebConfigWriter, IWebConfigReader, IDisposable
    {
        private const string ConnectionStringsSectionName = "connectionStrings";
        private const string AppSettingsSectionName = "appSettings";
        private const string CustomErrorsSectionName = "system.web/customErrors";

        private Configuration _webConfig;

        public WebConfigFileReaderWriter()
        {
            _webConfig = WebConfigurationManager.OpenWebConfiguration("~");
        }

        public IReadOnlyCollection<KeyValuePair<string, string>> GetAppSettings()
        {
            var appSettings = new List<KeyValuePair<string, string>>();
            foreach (KeyValueConfigurationElement appSetting in _webConfig.AppSettings.Settings)
            {
                appSettings.Add(new KeyValuePair<string, string>(appSetting.Key, appSetting.Value));
            }
            return appSettings.AsReadOnly();
        }

        public void SetCustomErrorsMode(CustomErrorsMode mode)
        {
            Debug.WriteLine($"Setting custom error mode: <customErrors mode=\"{mode}\"/>");
            var section = (CustomErrorsSection)_webConfig.GetSection(CustomErrorsSectionName);

            // Unfortunately this doesn't immediately take effect until the app is restarted
            // which means it requires a page refresh - which often times might be good enough
            section.Mode = mode;
        }

        public void SetConnectionString(string key, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException(nameof(key), "key cannot be empty, null, or all whitespace");
            }
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException(nameof(connectionString), "connectionString cannot be empty, null, or all whitespace");
            }
            Debug.WriteLine($"Adding connection string: <add name=\"{key}\" connectionString=\"{connectionString}\" />");
            SetConnectionStringValue(key, connectionString);
        }

        public void SetAppSetting(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException(nameof(key), "key cannot be empty, null, or all whitespace");
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value), "value cannot be null");
            }
            Debug.WriteLine($"Adding appSetting: <add key=\"{key}\" value=\"{value}\" />");
            SetAppSettingValue(key, value);
        }

        private void SetConnectionStringValue(string key, string connectionString)
        {
            var section = (ConnectionStringsSection)_webConfig.GetSection(ConnectionStringsSectionName);
            var connectionStringSetting = section.ConnectionStrings[key];
            if (connectionStringSetting != null)
            {
                section.ConnectionStrings.Remove(connectionStringSetting);
            }
            section.ConnectionStrings.Add(new ConnectionStringSettings(key, connectionString));
        }

        private void SetAppSettingValue(string key, string value)
        {
            var section = (AppSettingsSection)_webConfig.GetSection(AppSettingsSectionName);
            var appSetting = section.Settings[key];
            if (appSetting != null)
            {
                section.Settings.Remove(key);
            }
            section.Settings.Add(key, value);
        }

        public void Dispose()
        {
            _webConfig.Save();
            ConfigurationManager.RefreshSection(ConnectionStringsSectionName);
            ConfigurationManager.RefreshSection(AppSettingsSectionName);
            ConfigurationManager.RefreshSection(CustomErrorsSectionName);
        }
    }
}
