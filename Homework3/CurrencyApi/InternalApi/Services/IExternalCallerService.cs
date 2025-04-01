namespace InternalApi.Services
{
    public interface IExternalCallerService
    {
        Task<string> CallAsync(string uri, bool usesTockens = true);
    }
}
