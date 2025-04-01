namespace InternalApi.Models.Exceptions
{
    public class ApiRequestLimitException:Exception
    {
        public ApiRequestLimitException(string callerName)
    : base(message: callerName + " Ran out of tokens") { }
    }
}
