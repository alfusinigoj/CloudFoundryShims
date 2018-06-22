using System.Web.Configuration;

namespace PivotalServices.CloudFoundryShims
{
    public interface IWebConfigWriter
    {
        void SetConnectionString(string key, string connectionString);
        void SetAppSetting(string key, string value);
        void SetCustomErrorsMode(CustomErrorsMode mode);
    }
}
