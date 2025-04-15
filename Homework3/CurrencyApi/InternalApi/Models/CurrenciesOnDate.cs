namespace InternalApi.Models
{
    /// <summary>
    /// Курсы валют на конкретную дату
    /// </summary>
    public record CurrenciesOnDate
    {
        public DateTime LastUpdatedAt { get; set; }
        /// <summary>
        /// list of currencies
        /// </summary>
        public Currency[] Currencies { get; set; }
        public CurrenciesOnDate(DateTime lastUpdatedAt, Currency[] currencies)
        {
            LastUpdatedAt = lastUpdatedAt;
            Currencies = currencies;
        }
        public CurrenciesOnDate(ApiResponse apiResponse)
        {
            LastUpdatedAt = DateTime.Parse(apiResponse.Meta.LastUpdatedAt).ToUniversalTime();
            Currencies = apiResponse.Data.Values.Select(currencyEntry => new Currency(currencyEntry.Code, currencyEntry.Value)).ToArray();
        }
    }
}
