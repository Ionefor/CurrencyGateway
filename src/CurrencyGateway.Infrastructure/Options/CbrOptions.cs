namespace CurrencyGateway.Infrastructure.Options
{
    public class CbrOptions
    {
        public const string Cbr = nameof(Cbr);
        public string ApiUrl { get; set; } = string.Empty;
        public string CacheKeyPrefix { get; set; } = string.Empty;
        public string DefaultEncoding  { get; set; } = string.Empty;
        public int CacheTimeoutHours { get; set; }
        public int CbrUpdateHour { get; set; }
    }
}