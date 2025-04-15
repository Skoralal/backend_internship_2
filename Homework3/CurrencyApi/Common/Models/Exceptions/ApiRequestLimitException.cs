namespace Common.Models.Exceptions
{
    /// <summary>
    /// throw if there is no tokens left
    /// </summary>
    public class ApiRequestLimitException(string callerName) : Exception(message: callerName + " Ran out of tokens")
    {
    }
}
