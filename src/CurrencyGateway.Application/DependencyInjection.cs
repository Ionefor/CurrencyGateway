using CurrencyGateway.Application.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace CurrencyGateway.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<GetCurrencyHandler>();
        
            return services;
        }
    }
}