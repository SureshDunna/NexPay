using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace NexPay.Filters
{
    public class ExceptionLoggingFilter : IExceptionFilter
    {
        private readonly ILogger _logger;

        public ExceptionLoggingFilter(ILogger<ExceptionLoggingFilter> logger)
        {
            _logger = logger;
        }
        
        public void OnException(ExceptionContext context)
        {
            _logger.LogError(500, context.Exception, context.Exception.Message);
        }
    }
}