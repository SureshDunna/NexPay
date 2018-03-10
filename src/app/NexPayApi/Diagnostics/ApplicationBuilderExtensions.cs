using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace NexPayApi.Diagnostics
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseHealthCheck(this IApplicationBuilder app)
        {
            return app.UseHealthCheck(app.ApplicationServices.GetRequiredService<IHealthCheck>());
        }

        public static IApplicationBuilder UseHealthCheck(this IApplicationBuilder app, IHealthCheck healthCheck, string healthCheckRoute = "/healthcheck/ping")
        {
            return app.UseMiddleware<HealthCheckMiddleware>(new PathString(healthCheckRoute), healthCheck);
        }
    }
}