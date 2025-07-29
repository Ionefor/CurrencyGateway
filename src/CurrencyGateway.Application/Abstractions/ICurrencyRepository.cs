using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CurrencyGateway.Application.Models;
using CurrencyGateway.Domain.Models;

namespace CurrencyGateway.Application.Abstractions
{
    public interface ICurrencyService
    {
        Task<IReadOnlyList<Currency>> GetCurrencyRates(DateTime? date = null);
        Task<Currency> GetCurrencyRate(string currencyCode, DateTime? date = null);
    }
}