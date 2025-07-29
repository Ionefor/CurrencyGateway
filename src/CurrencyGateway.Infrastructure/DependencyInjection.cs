using System;
using CurrencyGateway.Application.Abstractions;
using CurrencyGateway.Infrastructure.Options;
using CurrencyGateway.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CurrencyGateway.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            
            services.AddHttpClient(CbrOptions.Cbr, client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
            });
            
            services.AddMemoryCache();
            
            services.Configure<CbrOptions>(
                configuration.GetSection(CbrOptions.Cbr));
            
            services.Configure<CurrencyOptions>(
                configuration.GetSection(CurrencyOptions.Currency));
            
            services.AddScoped<ICurrencyService, CbrCurrencyService>();
            
            return services;
        }
    }
}