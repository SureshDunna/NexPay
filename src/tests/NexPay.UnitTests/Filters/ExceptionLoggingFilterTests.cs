using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;
using NexPay.Filters;
using Xunit;

namespace NexPay.UnitTests.Filters
{
    public class ExceptionLoggingFilterTests
    {
        private readonly Mock<ILogger<ExceptionLoggingFilter>> _logger;
        private readonly ExceptionLoggingFilter _filter;

        public ExceptionLoggingFilterTests()
        {
            _logger = new Mock<ILogger<ExceptionLoggingFilter>>();
            _filter = new ExceptionLoggingFilter(_logger.Object);
        }

        [Fact]
        public void writes_exception_to_log_if_exception()
        {
            var exception = new Exception("test exception");
            _filter.OnException(new ExceptionContext(
                new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()), 
                new List<IFilterMetadata>()){Exception = exception});

            _logger.Verify(x => x.Log(LogLevel.Error, 500, It.IsAny<object>(), exception, It.IsAny<Func<object,Exception,string>>()), Times.Once);
        }
    }
}