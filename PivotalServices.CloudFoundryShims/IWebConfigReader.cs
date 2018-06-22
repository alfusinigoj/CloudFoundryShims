using System.Collections.Generic;

namespace PivotalServices.CloudFoundryShims
{
    public interface IWebConfigReader
    {
        IReadOnlyCollection<KeyValuePair<string, string>> GetAppSettings();
    }
}
