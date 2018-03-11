using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using NexPay.Extensions;

namespace NexPay.Diagnostics
{
    public class HealthCheckMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHealthCheck _healthCheck;
        private readonly string _healthCheckRoute;

        public HealthCheckMiddleware(RequestDelegate next, PathString healthCheckRoute, IHealthCheck healthCheck)
        {
            if(string.IsNullOrEmpty(healthCheckRoute))
            {
                throw new ArgumentException(nameof(healthCheckRoute));
            }

            _next = next;
            _healthCheck = healthCheck ?? throw new ArgumentException(nameof(healthCheck));
            _healthCheckRoute = healthCheckRoute;
        }

        public async Task Invoke(HttpContext context)
        {
            var requestUri = new Uri(context.Request.GetDisplayUrl());
            var healthCheckUri = new Uri($"{context.Request.Scheme}://{context.Request.Host}{context.Request.PathBase}{_healthCheckRoute}");

            if(Uri.Compare(requestUri, healthCheckUri, UriComponents.AbsoluteUri, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase) == 0)
            {
                await context.SetResponse(_healthCheck.CurrentHealthCheckResult, HttpStatusCode.OK);
                return;
            }

            await _next.Invoke(context);
        }
    }
}