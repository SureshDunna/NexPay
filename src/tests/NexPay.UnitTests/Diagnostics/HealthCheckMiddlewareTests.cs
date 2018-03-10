using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
using NexPayApi.Diagnostics;
using Xunit;

namespace NexPay.UnitTests.Diagnostics
{
    public class HealthCheckMiddlewareTests
    {
        private readonly Mock<IHealthCheck> _healthCheck;
        private readonly HealthCheckMiddleware _middleware;
        private readonly RequestDelegate _next;
        private readonly HttpContext _context;

        public HealthCheckMiddlewareTests()
        {
            _healthCheck = new Mock<IHealthCheck>();
            _context = new DefaultHttpContext();
            _next = TestInvoke;
            _middleware = new HealthCheckMiddleware(_next, new PathString("/healthcheck/ping"), _healthCheck.Object);
        }

        [Fact]
        public void throws_argumentexception_if_healthcheckroute_is_null_or_empty()
        {
            Assert.Throws<ArgumentException>(() => new HealthCheckMiddleware(null, null, null));
        }

        [Fact]
        public void throws_argumentexception_if_healthcheck_is_null_or_empty()
        {
            Assert.Throws<ArgumentException>(() => new HealthCheckMiddleware(null, new PathString("/"), null));
        }

        [Fact]
        public async Task returns_health_check_status_if_url_contains_healthcheck_ping()
        {
            _context.Request.Scheme = "http";
            _context.Request.Method = "GET";
            _context.Request.Path = "/healthcheck/ping";
            _context.Request.Host = new HostString("localhost");

            var healthcheckResult = new HealthCheckResult
            {
                AssemblyName = "HealthCheckMiddlewareTests",
                Version = "1.0",
                Healthy = true
            };

            _healthCheck.Setup(x => x.CurrentHealthCheckResult).Returns(healthcheckResult);

            var responseBody = string.Empty;

            using (var memStream = new MemoryStream()) 
            {
                _context.Response.Body = memStream;

                await _middleware.Invoke(_context);

                memStream.Position = 0;
                responseBody = new StreamReader(memStream).ReadToEnd();
            }

            Assert.NotNull(_context.Response);
            Assert.Equal(_context.Response.StatusCode, (int)HttpStatusCode.OK);
            Assert.Equal(responseBody, JsonConvert.SerializeObject(healthcheckResult));
        }

        [Fact]
        public async Task can_move_to_next_middleware_if_request_does_not_contain_healthcheck_url()
        {
            _context.Request.Scheme = "http";
            _context.Request.Method = "GET";
            _context.Request.Path = "/api/palindromes";
            _context.Request.Host = new HostString("localhost");

            var healthcheckResult = new HealthCheckResult
            {
                AssemblyName = "HealthCheckMiddlewareTests",
                Version = "1.0",
                Healthy = true
            };

            _healthCheck.Setup(x => x.CurrentHealthCheckResult).Returns(healthcheckResult);

            var responseBody = string.Empty;

            using (var memStream = new MemoryStream()) 
            {
                _context.Response.Body = memStream;

                await _middleware.Invoke(_context);

                memStream.Position = 0;
                responseBody = new StreamReader(memStream).ReadToEnd();
            }

            Assert.NotNull(_context.Response);
            Assert.Equal(_context.Response.StatusCode, (int)HttpStatusCode.Forbidden);
            Assert.NotEqual(responseBody, JsonConvert.SerializeObject(healthcheckResult));
        }

        private async Task TestInvoke(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            await Task.Delay(1);
        }
    }
}