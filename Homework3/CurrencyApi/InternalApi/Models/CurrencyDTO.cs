using Common.Models;

namespace InternalApi.Models
{
    /// <summary>
    /// Курс валюты
    /// </summary>
    public record CurrencyDTO
    {
        public CurrencyType CurrencyType { get; set; }
        /// <summary>
        /// exchange rate
        /// </summary>
        public decimal Value { get; set; }
        public CurrencyDTO(Currency currency)
        {
            CurrencyType = Enum.Parse<CurrencyType>(currency.Code);
            Value = currency.Value;
        }
        public CurrencyDTO() { }
    }
}
