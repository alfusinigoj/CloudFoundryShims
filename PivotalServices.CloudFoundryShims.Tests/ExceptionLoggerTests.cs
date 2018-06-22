using System;
using Xunit;

namespace PivotalServices.CloudFoundryShims.Tests
{
    public class ExceptionLoggerTests
    {
        [Fact]
        public void CanCreateLogMessageWithStackTrace()
        {
            var expectedErr =
@"Exception Type: Exception
Message: my ex msg
Source: PivotalServices.CloudFoundryShims.Tests
Stack Trace:    at PivotalServices.CloudFoundryShims.Tests.ExceptionLoggerTests.CreateException".Replace("\r\n", "\n");
            var err = ExceptionLogger.BuildErrorString(CreateException());

            Assert.StartsWith(expectedErr, err);
        }

        [Fact]
        public void CanCreateLogMessageWithInnerException()
        {
            var expectedOuterErr =
@"Exception Type: Exception
Message: outer
Source: PivotalServices.CloudFoundryShims.Tests
Stack Trace:    at PivotalServices.CloudFoundryShims.Tests.ExceptionLoggerTests.CreateException".Replace("\r\n", "\n");
            var expectedInnerErr =
@"Exception Type: Exception
Message: inner
Source: PivotalServices.CloudFoundryShims.Tests
Stack Trace:    at PivotalServices.CloudFoundryShims.Tests.ExceptionLoggerTests.CreateException".Replace("\r\n", "\n");
            var err = ExceptionLogger.BuildErrorString(CreateException("outer", CreateException("inner")));

            Assert.StartsWith(expectedOuterErr, err);
            Assert.Contains(expectedInnerErr, err);
        }

        private static Exception CreateException(string msg = "my ex msg", Exception inner = null)
        {
            try
            {
                throw new Exception(msg, inner);
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}
