using System.Collections.Generic;

namespace CurrencyGateway.Application.Models
{
    public class PageList<T>
    {
        public IReadOnlyList<T> Items { get; set; }
    
        public int TotalCount { get; set; }
    
        public int PageSize { get; set; }
    
        public int Page { get; set; }
    
        public bool HasPreviousPage => Page > 1;
    
        public bool HasNextPage => Page * PageSize < TotalCount;
    }
}