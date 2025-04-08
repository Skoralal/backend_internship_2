using Common.Models;
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
        private readonly IOptionsMonitor<AppOptions> _settings;
        public gRPCServer(ExternalCallerService callerService, IOptionsMonitor<AppOptions> settings)
        {
            _callerService = callerService;
            _settings = settings;
        }
        public override async Task<CurrencyDTOResponse> GetCurrencyOnDate(CurrencyOnDateRequest request, ServerCallContext context)
        {
            Models.CurrencyDTO currencyDTO = await _callerService.GetCurrencyOnDateAsync((CurrencyType)request.CurrencyType,
                (CurrencyType)request.BaseCurrencyType,  DateOnly.FromDateTime(new DateTime(request.Date, DateTimeKind.Utc)), context.CancellationToken);
            CurrencyDTOResponse response = new CurrencyDTOResponse()
            {
                CurrencyType = request.CurrencyType,
                Value = (double)currencyDTO.Value
            };
            return response;
        }

        public override async Task<CurrencyDTOResponse> GetCurrentCurrency(CurrentCurrencyRequest request, ServerCallContext context)
        {
            Models.CurrencyDTO currencyDTO = await _callerService.GetCurrentCurrencyAsync((CurrencyType)request.CurrencyType, (CurrencyType)request.BaseCurrencyType, context.CancellationToken);
            CurrencyDTOResponse response = new CurrencyDTOResponse()
            {
                CurrencyType = request.CurrencyType,
                Value = (double)currencyDTO.Value
            };
            return response;
        }
        public override async Task<gRPC.GrpcStatusResponse> GetStatus(Empty request, ServerCallContext context)
        {
            var status = new gRPC.GrpcStatusResponse()
            {
                HasRequests = await _callerService.HasTokens(),
            };
            return status;
        }
    }
}
