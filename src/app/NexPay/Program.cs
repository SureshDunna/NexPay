using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.PlatformAbstractions;
using NexPay.Filters;

namespace NexPayApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Build().Run();
        }

        public static IWebHostBuilder BuildWebHost(string[] args)
        {
           return new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseUrls("http://localhost:5001")
                .UseStartup<Startup>()
                .ConfigureServices((services) =>
                {
                    services.AddAutofac();
                    JsonConvert.DefaultSettings = () => new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        Converters = new List<JsonConverter> { new StringEnumConverter() }
                    };

                    services.AddMvc(
                        options =>
                        {
                            options.Filters.Add(typeof(ValidationFilterAttribute));
                            options.Filters.Add(typeof(ExceptionLoggingFilter));
                        }
                    )
                    .AddJsonOptions(
                        options => { options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); });
                    
                    services.AddSwaggerGen(options =>
                    {
                        options.SwaggerDoc("v1",
                        new Info
                        {
                            Title = "NexPay Api",
                            Version = "1.0"
                        });

                        options.IncludeXmlComments($"{PlatformServices.Default.Application.ApplicationBasePath}\\NexPay.xml");
                    });                
                });
        }
     }
}
