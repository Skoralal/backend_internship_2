namespace InternalApi.Models
{
    /// <summary>
    /// Курс валюты
    /// </summary>
    /// <param name="CurrencyType">Валюта</param>
    /// <param name="Value">Значение курса</param>
    public record CurrencyDTO
    {
        public CurrencyType CurrencyType { get; set; }
        /// <summary>
        /// exchange rate
        /// </summary>
        public decimal Value { get; set; }
        public CurrencyDTO()
        {

        }
        public CurrencyDTO(Currency currency)
        {
            CurrencyType = Enum.Parse<CurrencyType>(currency.Code);
            Value = currency.Value;
        }
    }
}
