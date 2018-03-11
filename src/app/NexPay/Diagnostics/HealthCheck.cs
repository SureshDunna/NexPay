using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace NexPay.Diagnostics
{
    public class HealthCheck : HealthCheckBase
    {
        private readonly ILogger _logger;
        private readonly IFileProvider _fileProvider;

        public HealthCheck(ILogger<HealthCheck> logger, IFileProvider fileProvider) : base(logger)
        {
            _logger = logger;
            _fileProvider = fileProvider;
        }

        protected override void ConfigureHealthChecks()
        {
            Register("File System Health Check", 
            () =>
            {
                try
                {
                    //Checking health check file and if it exists that means all set to accesss payment files
                    //in the same directory
                    return Task.FromResult(_fileProvider.GetFileInfo("healthcheck").Exists);
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