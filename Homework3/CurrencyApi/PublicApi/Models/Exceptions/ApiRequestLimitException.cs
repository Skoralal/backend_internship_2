namespace Fuse8.BackendInternship.PublicApi.Models.Exceptions
{
    public class ApiRequestLimitException:Exception
    {
        public ApiRequestLimitException(string callerName)
    : base(message: callerName + " Ran out of tokens") { }
    }
}
