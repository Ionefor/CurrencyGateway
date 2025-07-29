using System.Collections.Generic;
using System.Linq;
using CurrencyGateway.Application.Models;

namespace CurrencyGateway.Application.Extensions
{
    public static class ResponseExtensions
    {
        public static PageList<T> ToPagedList<T>(
            this IReadOnlyList<T> source,
            int page,
            int pageSize)
        {
            var totalCount =  source.Count;
        
            var items = source.
                Skip((page - 1) * pageSize).
                Take(pageSize).
                ToList();

            return new PageList<T>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
    }
}