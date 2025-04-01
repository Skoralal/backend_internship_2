using Google.Protobuf.WellKnownTypes;
using gRPC;
using Grpc.Core;
using InternalApi.Models;
using Microsoft.Extensions.Options;

namespace InternalApi.Services
{
    public class gRPCServer : gRPCCurrency.gRPCCurrencyBase
    {
        private readonly ExternalCallerService _callerService;
        private readonly IOptionsMonitor<DefaultSettings> _settings;
        public gRPCServer(ExternalCallerService callerService, IOptionsMonitor<DefaultSettings> settings)
        {
            _callerService = callerService;
            _settings = settings;
        }
        public override async Task<CurrencyDTOResponse> GetCurrencyOnDate(CurrencyOnDateRequest request, ServerCallContext context)
        {
            Models.CurrencyDTO currencyDTO = await _callerService.GetCurrencyOnDateAsync((Models.CurrencyType)request.CurrencyType, DateOnly.FromDateTime(new DateTime(request.Date, DateTimeKind.Utc)), CancellationToken.None);
            CurrencyDTOResponse response = new CurrencyDTOResponse()
            {
                CurrencyType = request.CurrencyType,
                Value = (double)currencyDTO.Value
            };
            return response;
        }

        public override async Task<CurrencyDTOResponse> GetCurrentCurrency(CurrentCurrencyRequest request, ServerCallContext context)
        {
            Models.CurrencyDTO currencyDTO = await _callerService.GetCurrentCurrencyAsync((Models.CurrencyType)request.CurrencyType, CancellationToken.None);
            CurrencyDTOResponse response = new CurrencyDTOResponse()
            {
                CurrencyType = request.CurrencyType,
                Value = (double)currencyDTO.Value
            };
            return response;
        }
        public override async Task<gRPC.Status> GetStatus(Empty request, ServerCallContext context)
        {
            var status = new gRPC.Status()
            {
                BaseCurrency = System.Enum.Parse<gRPC.CurrencyType>(_settings.CurrentValue.BaseCurrency, true),
                HasRequests = await _callerService.HasTokens(),
            };
            return status;
        }
    }
}
