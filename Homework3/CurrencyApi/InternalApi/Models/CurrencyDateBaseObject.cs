using Common.Models;

namespace InternalApi.Models
{
    public class CurrencyDateBaseObject
    {
        public decimal ExchangeRate {  get; set; }
        public long ActualityTime { get; set; }
        public string Code { get; set; }
        public CurrencyDateBaseObject(Currency currency, long TickActualityTime)
        {
            ExchangeRate = currency.Value;
            ActualityTime = TickActualityTime;
            Code = currency.Code;
        }
        public CurrencyDateBaseObject() { }
    }
}
