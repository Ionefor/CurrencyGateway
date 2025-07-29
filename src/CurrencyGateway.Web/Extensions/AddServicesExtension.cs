using System;
using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;
using CurrencyGateway.Application;
using CurrencyGateway.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

namespace CurrencyGateway.Web.Extensions
{
    public static class AddServicesExtension
    {
        public static IServiceCollection AddServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            
            services.AddInfrastructure(configuration);
            services.AddApplication();
            
            services.AddHealthChecks();
            
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Currency Gateway API",
                    Version = "v1",
                    Description = "API для получения курсов валют от Центрального банка России",
                    Contact = new OpenApiContact
                    {
                        Name = "Currency Gateway Team",
                        Email = "support@currencygateway.com"
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });
                
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
                
                options.CustomSchemaIds(type => type.FullName);
            });
            
           
            
            return services;
        }
    }
}