using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace NexPayApi.Extensions
{
    public static class HttpContextExtensions
    {
        public static async Task SetResponse<T>(this HttpContext context, T value, HttpStatusCode statusCode)
        {
            context.Response.StatusCode = (int)statusCode;
            await context.SetResponse(value);
        }

        public static async Task SetResponse<T>(this HttpContext context, T value)
        {
            var responseBody = JsonConvert.SerializeObject(value);
            context.Response.ContentType = new MediaTypeHeaderValue("application/json") { CharSet = "utf-8"}.ToString();
            await context.Response.WriteAsync(responseBody);
        }
    }
}