using Fuse8.BackendInternship.PublicApi.Models;
using gRPC;

namespace Fuse8.BackendInternship.PublicApi.Services
{
    public class GrpcCurrencyService
    {
        private readonly gRPCCurrency.gRPCCurrencyClient _client;
        public GrpcCurrencyService(gRPCCurrency.gRPCCurrencyClient client)
        {
            _client = client;
        }
        public async Task<CurrencyLoadBase> GetCurrentCurrency(string currencyCode)
        {
            CurrencyType currencyType = Enum.Parse<CurrencyType>(currencyCode, true);
            var dto = await _client.GetCurrentCurrencyAsync(new() { CurrencyType = currencyType});
            CurrencyLoadBase output = new()
            {
                Code = dto.CurrencyType.ToString(),
                Value = dto.Value,
            };
            return output;
        }

        public async Task<CurrencyLoadWDate> GetHistoricalCurrency(string currencyCode, DateOnly dateOnly)
        {
            CurrencyType currencyType = Enum.Parse<CurrencyType>(currencyCode, true);
            var dto = await _client.GetCurrencyOnDateAsync(new() { CurrencyType = currencyType, Date = dateOnly.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc).Ticks });
            CurrencyLoadWDate output = new()
            {
                Code = dto.CurrencyType.ToString(),
                Value = dto.Value,
                Date = dateOnly
            };
            return output;
        }

        public async Task<Status> GetStatus()
        {
            return await _client.GetStatusAsync(new Google.Protobuf.WellKnownTypes.Empty());
        }
    }
}
