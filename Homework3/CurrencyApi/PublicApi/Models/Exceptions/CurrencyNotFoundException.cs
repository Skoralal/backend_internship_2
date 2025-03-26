namespace Fuse8.BackendInternship.PublicApi.Models.Exceptions
{
    public class CurrencyNotFoundException : Exception
    {
        public CurrencyNotFoundException():base(message:"Could not find specified currency") { }
    }
}
