using System;
using Microsoft.Extensions.Configuration;
using System.Web.Configuration;

namespace PivotalServices.CloudFoundryShims
{
    public class CustomErrorsInitializer
    {
        private const string CustomErrorsModeEnvVarName = "CUSTOM_ERRORS_MODE";

        private readonly IConfigurationRoot _config;

        public CustomErrorsInitializer(IConfigurationRoot config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config), "The application config is required");
        }

        public void SetCustomErrorsMode(IWebConfigWriter webConfigWriter)
        {
            if (webConfigWriter == null)
            {
                throw new ArgumentNullException(nameof(webConfigWriter), "The web config writer is required");
            }

            var customErrorModeString = _config.GetValue<string>(CustomErrorsModeEnvVarName);
            if (!string.IsNullOrWhiteSpace(customErrorModeString))
            {
                CustomErrorsMode customErrorsMode;
                if (Enum.TryParse(customErrorModeString, true, out customErrorsMode))
                {
                    webConfigWriter.SetCustomErrorsMode(customErrorsMode);
                }
            }
        }
    }
}
