namespace Fuse8.BackendInternship.PublicApi.Models.Exceptions
{
    public class CrudOperationException:Exception
    {
        public CrudOperationException(string message) : base(message) { }
    }
}
