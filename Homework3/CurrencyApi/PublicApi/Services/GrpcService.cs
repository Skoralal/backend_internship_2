using Common.Models;
using Fuse8.BackendInternship.PublicApi.Models;
using Google.Protobuf.WellKnownTypes;
using gRPC;

namespace Fuse8.BackendInternship.PublicApi.Services
{
    /// <summary>
    /// service to call gRPC methods
    /// </summary>
    public class GrpcCurrencyService
    {
        private readonly gRPCCurrency.gRPCCurrencyClient _client;
        public GrpcCurrencyService(gRPCCurrency.gRPCCurrencyClient client)
        {
            _client = client;
        }
        /// <summary>
        /// get the latest exchange rate of specified currency pair
        /// </summary>
        /// <param name="currencyType">desired currency</param>
        /// <param name="baseCurrencyType">base currency</param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async Task<CurrencyLoadBase> GetCurrentCurrency(CurrencyType currencyType, CurrencyType baseCurrencyType, CancellationToken cancellation)
        {
            var dto = await _client.GetCurrentCurrencyAsync(new()
            {
                CurrencyType = (GrpcCurrencyType)currencyType,
                BaseCurrencyType = (GrpcCurrencyType)baseCurrencyType
            }, cancellationToken: cancellation);
            CurrencyLoadBase output = new()
            {
                Code = (CurrencyType)dto.CurrencyType,
                Value = dto.Value,
            };
            return output;
        }
        /// <summary>
        /// get the exchange rate of specified currency pair on specified date
        /// </summary>
        /// <param name="currencyType">desired currency</param>
        /// <param name="baseCurrencyType">>base currency</param>
        /// <param name="dateOnly">date the exhange rate was actual</param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async Task<CurrencyLoadWDate> GetHistoricalCurrency(CurrencyType currencyType, CurrencyType baseCurrencyType, DateOnly dateOnly, CancellationToken cancellation)
        {
            var dto = await _client.GetCurrencyOnDateAsync(new()
            {
                CurrencyType = (GrpcCurrencyType)currencyType,
                BaseCurrencyType = (GrpcCurrencyType)baseCurrencyType
                ,
                Date = dateOnly.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc).ToTimestamp()
            }, cancellationToken: cancellation);
            CurrencyLoadWDate output = new()
            {
                Code = (CurrencyType)dto.CurrencyType,
                Value = dto.Value,
                Date = dateOnly
            };
            return output;
        }
        /// <summary>
        /// get api status
        /// </summary>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async Task<GrpcStatusResponse> GetStatus(CancellationToken cancellation)
        {
            return await _client.GetStatusAsync(new Google.Protobuf.WellKnownTypes.Empty(), cancellationToken: cancellation);
        }
    }
}
