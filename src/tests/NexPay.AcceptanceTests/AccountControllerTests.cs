using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NexPay.Models;
using Xunit;

namespace NexPay.AcceptanceTests
{
    public class AccountControllerTests : TestBase, IDisposable
    {
        public void Dispose()
        {
            if(_server != null)
            {
                _server.Dispose();
            }
        }

        [Fact]
        public async Task can_retrieve_payments()
        {
            using(var client = _server.CreateClient())
            {
                var response = await client.GetAsync("http://localhost/api/account/paymentsto/1234");

                Assert.Equal(response.StatusCode, HttpStatusCode.OK);

                var payments = JsonConvert.DeserializeObject<IEnumerable<Payment>>(await response.Content.ReadAsStringAsync());

                Assert.NotNull(payments);
                Assert.True(payments.ToList().Count > 0);
                Assert.Contains(1234, payments.Select(x => x.AccNo));
            }
        }

        [Fact]
        public async Task can_transfer_payments()
        {
            using(var client = _server.CreateClient())
            {
                var response = await client.PostAsync("http://localhost/api/account/transfer",
                new StringContent(JsonConvert.SerializeObject(new Payment
                {
                    AccNo = 2345,
                    AccName = "NexPay",
                    Amount = 1000,
                    BSB = "062000",
                    PayRef = "Payments from Acceptance Test"
                }), Encoding.UTF8, "application/json"));

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                response = await client.GetAsync("http://localhost/api/account/paymentsto/2345");

                Assert.Equal(response.StatusCode, HttpStatusCode.OK);

                var payments = JsonConvert.DeserializeObject<IEnumerable<Payment>>(await response.Content.ReadAsStringAsync());

                Assert.NotNull(payments);
                Assert.True(payments.ToList().Count > 0);
                Assert.Contains(2345, payments.Select(x => x.AccNo));
            }
        }
    }
}