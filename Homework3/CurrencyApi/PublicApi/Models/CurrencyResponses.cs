namespace Fuse8.BackendInternship.PublicApi.Models
{
    /// <summary>
    /// Model exchange rates between two currencies w/o date
    /// </summary>
    public record CurrencyLoadBase
    {
        /// <summary>
        /// ISO 4217 currency code
        /// </summary>
        public string code { get; set; } = "";
        /// <summary>
        /// Exchange rate
        /// </summary>
        public double value { get; set; } = 0;
        public CurrencyLoadBase(ApiResponse response, int precision)
        {
            (this.code, this.value) = response.data.Values.First();
            value = Math.Round(value, precision);
        }
    }
    /// <summary>
    /// Model of exchange rates between two currencies w/ date
    /// </summary>
    public record CurrencyLoadWDate:CurrencyLoadBase
    {
        /// <summary>
        /// Date of exchange rate [format(YYYY-MM-DD)]
        /// </summary>
        public string date { get; set; } = "";
        public CurrencyLoadWDate(ApiResponse response, int precision) :base(response, precision)
        {
            date = response.meta.last_updated_at.ToString("yyyy-MM-dd");
        }
    }
}
