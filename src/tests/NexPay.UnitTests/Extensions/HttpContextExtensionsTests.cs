using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NexPayApi.Extensions;
using Xunit;

namespace NexPay.UnitTests.Extensions
{
    public class HttpContextExtensionsTests
    {
        [Fact]
        public async Task can_set_response_to_httpcontext()
        {
            using (var memStream = new MemoryStream()) 
            {
                var httpContext = new DefaultHttpContext();
                
                httpContext.Response.Body = memStream;

                var testObject = new
                {
                    Message = "test message"
                };

                await httpContext.SetResponse(testObject);

                memStream.Position = 0;
                var responseBody = new StreamReader(memStream).ReadToEnd();

                Assert.Equal(responseBody, JsonConvert.SerializeObject(testObject));
            }
        }

        [Fact]
        public async Task can_set_response_and_statuscode_to_httpcontext()
        {
            using (var memStream = new MemoryStream()) 
            {
                var httpContext = new DefaultHttpContext();
                
                httpContext.Response.Body = memStream;

                var testObject = new
                {
                    Message = "test message"
                };

                await httpContext.SetResponse(testObject, HttpStatusCode.OK);

                memStream.Position = 0;
                var responseBody = new StreamReader(memStream).ReadToEnd();

                Assert.Equal(responseBody, JsonConvert.SerializeObject(testObject));
                Assert.Equal(httpContext.Response.StatusCode, (int)HttpStatusCode.OK);
            }
        }
    }
}