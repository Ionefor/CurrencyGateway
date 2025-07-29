using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using CurrencyGateway.Application.Abstractions;
using CurrencyGateway.Application.Extensions;
using CurrencyGateway.Application.Models;
using CurrencyGateway.Application.Requests;
using CurrencyGateway.Domain.Models;

namespace CurrencyGateway.Application.Handlers
{
    public class GetCurrencyHandler
    {
        private readonly ICurrencyService _currencyService;
        
        public GetCurrencyHandler(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }
        
        public async Task<Result<PageList<Currency>>> Handle(
            GetCurrencyRequest request)
        {
            if (request.CurrencyCode is null)
                return await GetCurrencies(request);
            
            return await GetCurrency(request);
        }

        private async Task<Result<PageList<Currency>>> GetCurrencies(GetCurrencyRequest request)
        {
            var currencies = await _currencyService.
                GetCurrencyRates(request.Date);
                
            return currencies.ToPagedList(
                request.PaginationParams!.Page,
                request.PaginationParams.PageSize);
        }
        
        private async Task<Result<PageList<Currency>>> GetCurrency(GetCurrencyRequest request)
        {
            var currency = await _currencyService.
                GetCurrencyRate(request.CurrencyCode, request.Date);
            
            if(currency is null)
                return Result.Failure<PageList<Currency>>("Currency not found");
            
            var currencyList = new List<Currency> { currency};
            
            return currencyList.ToPagedList(
                request.PaginationParams.Page,
                request.PaginationParams.PageSize);
        }
    }
}