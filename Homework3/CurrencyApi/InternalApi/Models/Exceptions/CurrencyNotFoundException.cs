namespace InternalApi.Models.Exceptions
{
    public class CurrencyNotFoundException : Exception
    {
        public CurrencyNotFoundException():base(message:"Could not find specified currency") { }
    }
}
