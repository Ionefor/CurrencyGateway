using System;

namespace CurrencyGateway.Domain.Models
{
    public class Currency
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Rate { get; set; }
        public int Nominal { get; set; }
        public DateTime Date { get; set; }
    }
}