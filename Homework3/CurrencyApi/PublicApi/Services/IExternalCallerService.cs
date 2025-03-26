namespace Fuse8.BackendInternship.PublicApi.Services
{
    public interface IExternalCallerService
    {
        Task<string> CallAsync(string uri, bool usesTockens = true);
    }
}
