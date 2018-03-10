using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Moq;
using NexPayApi.BusinessLogic;
using NexPayApi.Models;
using Xunit;

namespace NexPay.UnitTests.BusinessLogic
{
    public class PaymentRepositoryTests
    {
        private readonly Mock<IFileProvider> _fileProvider;
        private readonly Mock<IFileInfo> _fileInfo;
        private readonly IPaymentRepository _paymentRepository;

        public PaymentRepositoryTests()
        {
            _fileProvider = new Mock<IFileProvider>();
            _fileInfo = new Mock<IFileInfo>();
            _fileProvider.Setup(x => x.GetFileInfo(It.IsAny<string>())).Returns(_fileInfo.Object);
            _paymentRepository = new PaymentRepository(_fileProvider.Object, new LoggerFactory().CreateLogger<PaymentRepository>());
        }

        [Fact]
        public async Task can_retrieve_payments()
        {
            _fileInfo.Setup(x => x.Exists).Returns(true);
            _fileInfo.Setup(x => x.CreateReadStream()).Returns(GenerateStreamFromString("[{\"bsb\":\"062000\",\"accountNumber\":2345,\"accountName\":\"NexPay\",\"reference\":\"Payments from Acceptance Test\",\"amount\":1000.0}]"));
            var payments = await _paymentRepository.GetPaymentsAsync(2345);

            Assert.NotNull(payments);
            Assert.True(payments.ToList().Count > 0);
            Assert.Contains(2345, payments.Select(x => x.AccountNumber));
        }

        [Fact]
        public async Task returns_null_if_file_not_found()
        {
            var payments = await _paymentRepository.GetPaymentsAsync(2345);

            Assert.Null(payments);
        }

        [Fact]
        public async Task returns_null_if_empty_data_found()
        {
            _fileInfo.Setup(x => x.Exists).Returns(true);
            _fileInfo.Setup(x => x.CreateReadStream()).Returns(GenerateStreamFromString(string.Empty));
            var payments = await _paymentRepository.GetPaymentsAsync(2345);

            Assert.Null(payments);
        }

        [Fact]
        public async Task can_update_payments()
        {
            _fileInfo.Setup(x => x.Exists).Returns(true);
            _fileInfo.Setup(x => x.CreateReadStream()).Returns(GenerateStreamFromString("[{\"bsb\":\"062000\",\"accountNumber\":2345,\"accountName\":\"NexPay\",\"reference\":\"Payments from Acceptance Test\",\"amount\":1000.0}]"));
            _fileInfo.Setup(x => x.PhysicalPath).Returns(Path.Combine(Directory.GetCurrentDirectory(), "Payment_2345"));
            await _paymentRepository.WritePaymentAsync(new Payment{
               AccountNumber = 2345,
               Amount = 10000 
            });

            _fileInfo.Setup(x => x.CreateReadStream()).Returns(GenerateStreamFromString(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Payment_2345"))));
            
            var payments = await _paymentRepository.GetPaymentsAsync(2345);

            Assert.NotNull(payments);
            Assert.Equal(2, payments.ToList().Count);
        }

        private static Stream GenerateStreamFromString(string stringValue)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(stringValue);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}