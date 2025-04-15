namespace Fuse8.BackendInternship.PublicApi.Models.Exceptions
{
    /// <summary>
    /// throw if crud operation failed
    /// </summary>
    public class CrudOperationException : Exception
    {
        public CrudOperationException(string message) : base(message) { }
    }
}
