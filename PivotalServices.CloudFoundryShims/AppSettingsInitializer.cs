using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace PivotalServices.CloudFoundryShims
{
    /// <summary>
    /// Initializes appSettings section of the web.config from CF config sources
    /// </summary>
    public class AppSettingsInitializer
    {
        private readonly IConfigurationRoot _config;
        private readonly IWebConfigReader _webConfigReader;

        public AppSettingsInitializer(IConfigurationRoot config, IWebConfigReader webConfigReader)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config), "The application config is required");
            _webConfigReader = webConfigReader ?? throw new ArgumentNullException(nameof(webConfigReader), "The webConfigReader is required");
        }

        public void CopyConfigToAppSettings(IWebConfigWriter webConfigWriter)
        {
            if (webConfigWriter == null)
            {
                throw new ArgumentNullException(nameof(webConfigWriter), "The web config writer is required");
            }

            foreach (KeyValuePair<string, string> pair in _webConfigReader.GetAppSettings())
            {
                var updatedValue = _config.GetValue<string>(pair.Key);
                if (updatedValue != null)
                {
                    webConfigWriter.SetAppSetting(pair.Key, updatedValue);
                }
            }
        }
    }
}
