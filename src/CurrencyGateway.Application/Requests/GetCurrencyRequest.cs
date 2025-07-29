#nullable enable
using System;
using CurrencyGateway.Application.Dtos;
using CurrencyGateway.Application.Models;

namespace CurrencyGateway.Application.Requests
{
    public class GetCurrencyRequest
    {
        public DateTime? Date { get; set; }
        public string? CurrencyCode { get; set; }
        public PaginationParamsDto PaginationParams { get; set; }
    }
}