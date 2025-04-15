using Common.Models;

namespace InternalApi.Models
{
    public class CurrencyDB
    {
        public Guid Id { get; set; }
        /// <summary>
        /// exchange rate against the base currency
        /// </summary>
        public decimal ExchangeRate { get; set; }
        /// <summary>
        /// exact time of when data was actual
        /// </summary>
        public DateTime ActualityTime { get; set; }
        /// <summary>
        /// currency code as universal enum
        /// </summary>
        public CurrencyType Code { get; set; }
        public CurrencyDB(Currency currency, DateTime TickActualityTime)
        {
            ExchangeRate = currency.Value;
            ActualityTime = TickActualityTime;
            if (Enum.TryParse(currency.Code, out CurrencyType parsed))
            {
                Code = parsed;
            }
            else
            {
                Code = CurrencyType.NotSet;
            }
        }
        public CurrencyDB() { }
    }
}
