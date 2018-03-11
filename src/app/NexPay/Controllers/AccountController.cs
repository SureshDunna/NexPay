using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NexPay.BusinessLogic;
using NexPay.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NexPay.Controllers
{
    [Route("api/account")]
    public class AccountController : Controller
    {
        private readonly IPaymentRepository _paymentRepository;

        public AccountController(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        [HttpGet("paymentsto/{accountNumber}")]
        [ProducesResponseType(typeof(IEnumerable<Payment>), 200)]
        [SwaggerOperation(operationId: "GetPayments")]
        public async Task<IActionResult> GetPayments(long accountNumber)
        {
            return Ok(await _paymentRepository.GetPaymentsAsync(accountNumber));
        }

        [HttpPost("transfer")]
        [ProducesResponseType(typeof(void), 200)]
        [SwaggerOperation(operationId: "Transfer")]
        public async Task<IActionResult> Transfer([FromBody, Required] Payment payment)
        {
            await _paymentRepository.WritePaymentAsync(payment);

            return Ok();
        }
    }
}