using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NexPayApi.Diagnostics
{
    public interface IHealthCheck
    {
        HealthCheckResult CurrentHealthCheckResult { get; }
    }
    public abstract class HealthCheckBase : IHealthCheck
    {
        private static readonly TimeSpan DefaultScheduleInterval = TimeSpan.FromMinutes(5);
        private readonly ILogger _logger;
        private readonly IList<KeyValuePair<string, Func<Task<bool>>>> _registrations;
        private readonly TimeSpan _scheduleInterval;

        public HealthCheckResult CurrentHealthCheckResult { get; private set; }

        protected HealthCheckBase(ILogger logger) : this(logger, DefaultScheduleInterval)
        {}

        protected HealthCheckBase(ILogger logger, TimeSpan healthCheckInterval)
        {
            _registrations = new List<KeyValuePair<string, Func<Task<bool>>>>();
            _logger = logger;
            _scheduleInterval = healthCheckInterval;

            ConfigureHealthChecks();
            Task.Run(ApplicationStartUp);
        }

        public virtual async Task ApplicationStartUp()
        {
            await ExecuteHealthCheck("Start up");

            if(_scheduleInterval != TimeSpan.Zero)
            {
                await ScheduleHealthCheckExecution(_scheduleInterval);
            }
        }

        /// <summary>
        /// Registers all health check calls to be performed
        /// </summary>
        protected abstract void ConfigureHealthChecks();

        protected void Register(string description, Func<Task<bool>> healthCheckPredicate)
        {
            _registrations.Add(new KeyValuePair<string, Func<Task<bool>>>(description, healthCheckPredicate));
        }

        private async Task ExecuteHealthCheck(string messagePrefix)
        {
            var errors = new List<string>();

            foreach(var registration in _registrations)
            {
                try
                {
                    var success = await registration.Value();

                    if(success)
                    {
                        _logger.LogInformation($"{messagePrefix}: Health check successful: {registration.Key}");
                    }
                    else
                    {
                        var errorMessage = $"{messagePrefix}: Health check failed: {registration.Key}";

                        errors.Add(errorMessage);
                        _logger.LogError(errorMessage);
                    }
                }
                catch(Exception ex)
                {
                    var errorMessage = $"{messagePrefix}: Health check failed: {registration.Key}";

                    errors.Add(errorMessage);
                    _logger.LogError(500, ex, errorMessage);
                }
            }

            var entryAssembly = Assembly.GetEntryAssembly().GetName();

            CurrentHealthCheckResult = new HealthCheckResult
            {
                AssemblyName = entryAssembly.FullName,
                Version = entryAssembly.Version.ToString(),
                Healthy = !errors.Any()
            };
        }

        private async Task ScheduleHealthCheckExecution(TimeSpan interval)
        {
            while(true)
            {
                await Task.Delay(interval);
                await ExecuteHealthCheck("Scheduled");
            }
        }
    }
}