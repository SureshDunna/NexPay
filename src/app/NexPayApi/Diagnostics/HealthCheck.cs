using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NexPayApi.Diagnostics
{
    public class HealthCheck : HealthCheckBase
    {
        private readonly ILogger _logger;

        public HealthCheck(ILogger<HealthCheck> logger) : base(logger)
        {
            _logger = logger;
        }

        protected override void ConfigureHealthChecks()
        {
            Register("DB Health Check", 
            () =>
            {
                try
                {
                    //TODO: Implement Healthcheck
                    return Task.FromResult(true);
                }
                catch(Exception ex)
                {
                    _logger.LogError($"Error occured while doing the healthcheck {ex}");
                    return Task.FromResult(false);
                }
            });
        }
    }
}