using Common.Models;
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
        public async Task<CurrencyLoadBase> GetCurrentCurrency(CurrencyType currencyType, byte precision, CancellationToken cancellation)
        {
            var dto = await _client.GetCurrentCurrencyAsync(new() { CurrencyType = (GrpcCurrencyType)currencyType}, cancellationToken:cancellation);
            CurrencyLoadBase output = new()
            {
                Code = (CurrencyType)dto.CurrencyType,
                Value = Math.Round(dto.Value, precision),
            };
            return output;
        }

        public async Task<CurrencyLoadWDate> GetHistoricalCurrency(CurrencyType currencyType, DateOnly dateOnly, byte precision, CancellationToken cancellation)
        {
            var dto = await _client.GetCurrencyOnDateAsync(new() { CurrencyType = (GrpcCurrencyType)currencyType
                , Date = dateOnly.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc).Ticks }, cancellationToken: cancellation);
            CurrencyLoadWDate output = new()
            {
                Code = (CurrencyType)dto.CurrencyType,
                Value = Math.Round(dto.Value, precision),
                Date = dateOnly
            };
            return output;
        }

        public async Task<GrpcStatusResponse> GetStatus(CancellationToken cancellation)
        {
            return await _client.GetStatusAsync(new Google.Protobuf.WellKnownTypes.Empty(), cancellationToken: cancellation);
        }
    }
}
