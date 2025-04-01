namespace InternalApi.Models
{
    public class Status
    {
        /// <summary>
        /// can make a request or not
        /// </summary>
        public bool HasRequests { get; set; }
        /// <summary>
        /// what is the base currency
        /// </summary>
        public CurrencyType BaseCurrency { get; set; }
    }
}
