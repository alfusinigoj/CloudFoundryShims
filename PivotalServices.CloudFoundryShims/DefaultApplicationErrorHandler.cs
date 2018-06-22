using System;
using System.Web;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;

namespace PivotalServices.CloudFoundryShims
{
    /// <summary>
    /// Default error handler for ASP.NET exceptions that logs to stdout
    /// </summary>
    /// <remarks>
    /// This may not log errors that occur _really_ early in the ASP.NET
    /// pipeline, for example in Application Start.
    /// </remarks>
    public class DefaultApplicationErrorHandler : IHttpModule
    {
        public static void Register()
        {
            DynamicModuleUtility.RegisterModule(typeof(DefaultApplicationErrorHandler));
        }

        public void Init(HttpApplication context)
        {
            context.Error += ApplicationError;
        }

        private void ApplicationError(object sender, EventArgs e)
        {
            // get the ex but don't clear it, let the application clear it if desired
            var ex = HttpContext.Current.Server.GetLastError();
            if (ExIsLogWorthy(ex))
            {
                ExceptionLogger.LogError(ex);
            }
        }

        private static bool ExIsLogWorthy(Exception ex)
        {
            return ex != null && ex.GetType() != typeof(HttpException);
        }

        public void Dispose() { }
    }
}
