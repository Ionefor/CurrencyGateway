#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using CurrencyGateway.Application.Abstractions;
using CurrencyGateway.Domain.Models;
using CurrencyGateway.Infrastructure.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CurrencyGateway.Infrastructure.Services
{
    public class CbrCurrencyService : ICurrencyService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CbrCurrencyService> _logger;
        private readonly CbrOptions _cbrOptions;
        private readonly CurrencyOptions _currencyOptions;
        
        public CbrCurrencyService(
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache,
            ILogger<CbrCurrencyService> logger,
            IOptions<CbrOptions> cbrOptions,
            IOptions<CurrencyOptions> currencyOptions)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _logger = logger;
            _cbrOptions = cbrOptions.Value;
            _currencyOptions = currencyOptions.Value;
        }

        public async Task<IReadOnlyList<Currency>> GetCurrencyRates(DateTime? date = null)
        {
            var targetDate = date ?? DateTime.Today;
            
            var cachedCurrencies = TryGetFromCache(targetDate);
            
            if (cachedCurrencies != null)
                return cachedCurrencies;

            try
            {
                var currencies = await FetchCurrenciesFromCbr(targetDate);
                
                SetToCache(targetDate, currencies);

                _logger.LogInformation(
                        "Получено {Count} курсов валют для даты {Date}",
                        currencies.Count, targetDate);

                return currencies;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, "Ошибка при получении курсов валют для даты {Date}",
                    targetDate);
                
                throw;
            }
        }
        
        public async Task<Currency?> GetCurrencyRate(string currencyCode, DateTime? date = null)
        {
            var allCurrencyRates = await GetCurrencyRates(date);

            var currency = allCurrencyRates.
                FirstOrDefault(c => string.Equals(
                    c.Code, currencyCode, StringComparison.OrdinalIgnoreCase));

            if (currency is null)
                _logger.LogWarning($"Валюта {currencyCode} не найдена");
            
            return currency;
        }
        private IReadOnlyList<Currency>? TryGetFromCache(DateTime targetDate)
        {
            var cacheKey = $"{_cbrOptions.CacheKeyPrefix}{targetDate:yyyyMMdd}";
    
            if (_cache.TryGetValue(cacheKey, out IReadOnlyList<Currency> cachedCurrencies))
            {
                _logger.LogInformation(
                    "Курсы валют получены из кеша для даты {Date}", targetDate);
                
                return cachedCurrencies;
            }

            return null;
        }
        
        private void SetToCache(DateTime targetDate, IReadOnlyList<Currency> currencies)
        {
            var cacheKey = $"{_cbrOptions.CacheKeyPrefix}{targetDate:yyyyMMdd}";
            
            var cacheExpiry = CalculateCacheExpiryUntilNextNoon();
    
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = cacheExpiry,
                Priority = CacheItemPriority.High
            };

            _cache.Set(cacheKey, currencies, cacheOptions);
    
            _logger.LogInformation(
                "Данные сохранены в кеш до {CacheExpiry}", cacheExpiry);
        }
        private DateTime CalculateCacheExpiryUntilNextNoon()
        {
            var todayTimeCache = DateTime.
                Today.AddHours(_cbrOptions.CbrUpdateHour);

            return DateTime.Now < todayTimeCache 
                ? todayTimeCache 
                : DateTime.Today.AddDays(1).AddHours(_cbrOptions.CbrUpdateHour);
        }
        private async Task<IReadOnlyList<Currency>> FetchCurrenciesFromCbr(DateTime date)
        {
            var url = $"{_cbrOptions.ApiUrl}?date_req={date:dd/MM/yyyy}";
    
            using var httpClient = _httpClientFactory.CreateClient();
            
            using var response = await httpClient.GetAsync(url);
            
            response.EnsureSuccessStatusCode();
            
            var responseBytes = await response.Content.ReadAsByteArrayAsync();
    
            var xmlResponse = DecodeResponseContent(
                responseBytes, response.Content.Headers.ContentType?.CharSet);
            
            return ParseCbrXmlResponse(xmlResponse, date);
        }
        
        private string DecodeResponseContent(byte[] responseBytes, string? charset)
        {
            try
            {
                if (!string.IsNullOrEmpty(charset))
                {
                    var encoding = System.Text.Encoding.GetEncoding(charset);
                    return encoding.GetString(responseBytes);
                }
               
                return System.Text.Encoding.
                    GetEncoding(_cbrOptions.DefaultEncoding).GetString(responseBytes);
            }
            catch (ArgumentException)
            {
                return System.Text.Encoding.UTF8.GetString(responseBytes);
            }
        }
        
        private IReadOnlyList<Currency> ParseCbrXmlResponse(string xmlContent, DateTime date)
        {
            var document = XDocument.Parse(xmlContent);
            var currencies = new List<Currency>();

            foreach (var currency in document.Descendants())
            {
                try
                {
                    var code = currency.Element(_currencyOptions.CharCode)?.Value;
                    var name = currency.Element(_currencyOptions.Name)?.Value;
                    var nominalText = currency.Element(_currencyOptions.Nominal)?.Value;
                    var valueText = currency.Element(_currencyOptions.Value)?.Value;

                    if (string.IsNullOrEmpty(code)
                        || string.IsNullOrEmpty(valueText)
                        || string.IsNullOrEmpty(nominalText))
                    {
                        continue;
                    }
                    
                    if (int.TryParse(nominalText, out var nominal) &&
                        decimal.TryParse(valueText.Replace(',', '.'), NumberStyles.AllowDecimalPoint,
                            CultureInfo.InvariantCulture, out var rate))
                    {
                        currencies.Add(new Currency
                        {
                            Code = code,
                            Name = name,
                            Nominal = nominal,
                            Rate = rate,
                            Date = date
                        });
                        
                        _logger.LogDebug(
                            "Парсинг валюты {Code}: {Rate} руб за {Nominal} единиц выполнен успешно", 
                            code, rate, nominal);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
                        ex, "Не удалось выполнить парсинг валюты");
                    
                    continue;
                }
            }

            return currencies.AsReadOnly();
        }
    }
}