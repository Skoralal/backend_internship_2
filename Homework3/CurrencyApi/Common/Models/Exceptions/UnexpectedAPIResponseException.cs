using System.Net;

namespace Common.Models.Exceptions
{
    /// <summary>
    /// throw if API responded with an unexpected code
    /// </summary>
    public class UnexpectedAPIResponseException(HttpStatusCode code) : Exception(message: $"Api responded with an unexpected code: {code}")
    {
    }
}
