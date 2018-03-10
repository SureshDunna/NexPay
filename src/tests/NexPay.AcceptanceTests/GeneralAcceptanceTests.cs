using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace NexPay.AcceptanceTests
{
    public class GeneralAcceptanceTests : TestBase, IDisposable
    {
        [Fact]
        public async Task swagger_doc_is_enabled()
        {
            var swaggerDoc = await _server.CreateClient().GetAsync("http://localhost/swagger/v1/swagger.json");
            
            Assert.Equal(swaggerDoc.StatusCode, HttpStatusCode.OK);
        }

        [Fact]
        public async Task swagger_ui_is_enabled()
        {
            var response = await _server.CreateClient().GetAsync("http://localhost/swagger/");
            
            Assert.Equal(response.StatusCode, HttpStatusCode.OK);
        }

        [Fact]
        public async Task healthcheck_is_enabled()
        {
            var response = await _server.CreateClient().GetAsync("http://localhost/healthcheck/ping");

            Assert.Equal(response.StatusCode, HttpStatusCode.OK);
        }
        public void Dispose()
        {
            if(_server != null)
            {
                _server.Dispose();
            }
        }
    }
}