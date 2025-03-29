using System.Net;

namespace Fuse8.BackendInternship.PublicApi.Models.Exceptions
{
    public class UnexpectedAPIResponseException:Exception
    {
        public UnexpectedAPIResponseException(HttpStatusCode code):base(message:$"Api responded with an unexpected code: {code}") { }
    }
}
