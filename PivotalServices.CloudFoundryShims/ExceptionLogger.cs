using System;
using System.Diagnostics;
using System.Text;

namespace PivotalServices.CloudFoundryShims
{
    public static class ExceptionLogger
    {
        private const int MaxExceptionsToUnroll = 10;

        public static void LogError(Exception ex)
        {
            var err = TryBuildErrorString(ex);
            Console.Error.Write(err);
            if (Debugger.IsAttached)
            {
                // print the error to VS debug output
                Debug.Write(err);
            }
        }

        public static string TryBuildErrorString(Exception ex)
        {
            try
            {
                return BuildErrorString(ex);
            }
            catch (Exception e)
            {
                Debug.Write(e);
                return "Failed to build exception string";
            }
        }

        public static string BuildErrorString(Exception ex)
        {
            var err = new StringBuilder();
            AppendException(err, ex);

            int innerExCount = 0;
            while (ex.InnerException != null && innerExCount < MaxExceptionsToUnroll)
            {
                ex = ex.InnerException;
                AppendException(err, ex);
                innerExCount++;
            }

            if (innerExCount == MaxExceptionsToUnroll)
            {
                err.Append($"Unrolling inner exceptions stopped at {innerExCount}, you may be missing exceptions\n");
            }

            return err.ToString();
        }

        private static void AppendException(StringBuilder err, Exception ex)
        {
            err.Append($"Exception Type: {ex.GetType().Name}\n");
            err.Append($"Message: {ex.Message}\n");
            err.Append($"Source: {ex.Source}\n");
            if (ex.StackTrace != null)
            {
                err.Append($"Stack Trace: {ex.StackTrace}\n");
            }
        }
    }
}
