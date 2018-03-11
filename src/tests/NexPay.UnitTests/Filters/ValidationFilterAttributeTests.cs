using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using NexPay.Filters;
using Xunit;

namespace NexPay.UnitTests.Filters
{
    public class ValidationFilterAttributeTests
    {
        private readonly ValidationFilterAttribute _validationFilter;
        private readonly Mock<ILogger<ValidationFilterAttribute>> _logger;

        public ValidationFilterAttributeTests()
        {
            _logger = new Mock<ILogger<ValidationFilterAttribute>>();
            _validationFilter = new ValidationFilterAttribute(_logger.Object);
        }

        [Fact]
        public void returns_ok_if_modelstate_is_valid()
        {
            var context = new ActionExecutingContext(
                new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
                new List<IFilterMetadata>(), new Dictionary<string, object>(), new TestController());

            _validationFilter.OnActionExecuting(context);

            Assert.Null(context.Result);
        }

        [Fact]
        public void returns_bad_request_if_modelstate_is_not_valid()
        {
            var controller = new TestController();
            var context = new ActionExecutingContext(
                new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
                new List<IFilterMetadata>(), new Dictionary<string, object>(), controller);

            controller.ViewData.ModelState.AddModelError("xyz", "model error from unit test");

            _validationFilter.OnActionExecuting(context);

            Assert.NotNull(context.Result);

            var objectResult = (ObjectResult)context.Result;

            Assert.Equal(objectResult.StatusCode, (int)HttpStatusCode.BadRequest);
        }
    }

    public class TestController : Controller
    {
    }
}