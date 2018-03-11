using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace NexPay.Filters
{
    public class ValidationFilterAttribute : ActionFilterAttribute
    {
        private readonly ILogger _logger;

        public ValidationFilterAttribute(ILogger<ValidationFilterAttribute> logger)
        {
            _logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var viewData = ((Controller)context.Controller).ViewData;
            var modelStateDictionary = viewData.ModelState;

            if(!modelStateDictionary.IsValid)
            {
                _logger.LogInformation($"ModelState validation error. ModelState: {JsonConvert.SerializeObject(modelStateDictionary)}");

                var modelStateMessage = modelStateDictionary
                .Where(modelState => modelState.Value.Errors != null && modelState.Value.Errors.Any())
                .ToDictionary(modelState => modelState.Key, modelState => modelState.Value.Errors.Select(e => e.ErrorMessage));

                var response = new ValidationFailure
                {
                    Message = "The request is invalid",
                    ModelState = modelStateMessage
                };

                context.Result = new ObjectResult(response)
                {
                    StatusCode = 400
                };
            }
        }
    }
}