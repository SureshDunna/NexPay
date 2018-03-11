using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NexPay.Models;

namespace NexPay.BusinessLogic
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<Payment>> GetPaymentsAsync(long accountNumber);
        Task WritePaymentAsync(Payment paymentRequest);
    }
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IFileProvider _fileProvider;

        private readonly ILogger _logger;

        public PaymentRepository(IFileProvider fileProvider, ILogger<PaymentRepository> logger)
        {
            _fileProvider = fileProvider;
            _logger = logger;
        }

        public async Task<IEnumerable<Payment>> GetPaymentsAsync(long accountNumber)
        {
            var fileName = $"Payment_{accountNumber}";

            var paymentFile = _fileProvider.GetFileInfo(fileName);

            if(paymentFile == null || !paymentFile.Exists)
            {
                _logger.LogInformation($"{fileName} not found");
                return null;
            }   

            using (var stream = paymentFile.CreateReadStream())
            using (var reader = new StreamReader(stream))
            {
                var paymentData = await reader.ReadToEndAsync();
               
                if(string.IsNullOrWhiteSpace(paymentData))
                {
                    _logger.LogInformation($"{fileName} has empty data");
                    return null;
                }
            
                return JsonConvert.DeserializeObject<IEnumerable<Payment>>(paymentData);
            }
            
        }

        public async Task WritePaymentAsync(Payment paymentRequest)
        {
            var currentPayments = await GetPaymentsAsync(paymentRequest.AccNo);

            var updatedPayments = new List<Payment>();

            if(currentPayments != null)
            {
                updatedPayments.AddRange(currentPayments);
            }

            updatedPayments.Add(paymentRequest);

            var fileName = $"Payment_{paymentRequest.AccNo}";            

            using (var writer = File.CreateText(_fileProvider.GetFileInfo(fileName).PhysicalPath))
            {
                var payments = JsonConvert.SerializeObject(updatedPayments);

                await writer.WriteAsync(payments);
            }            
        }
    }
}