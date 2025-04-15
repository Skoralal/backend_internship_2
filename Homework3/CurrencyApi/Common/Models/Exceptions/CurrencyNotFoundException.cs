namespace Common.Models.Exceptions
{
    /// <summary>
    /// throw if a currency is not found
    /// </summary>
    public class CurrencyNotFoundException : Exception
    {
        public CurrencyNotFoundException() : base(message: "Could not find specified currency") { }
        public CurrencyNotFoundException(string message) : base(message: message) { }
    }
}
