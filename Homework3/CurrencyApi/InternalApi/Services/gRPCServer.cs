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
        private readonly CurrencyRequestHandlerService _callerService;
        private readonly HttpCallerService _httpService;
        public gRPCServer(CurrencyRequestHandlerService callerService, HttpCallerService httpService, IOptionsMonitor<AppOptions> settings)
        {
            _callerService = callerService;
            _httpService = httpService;
        }
        public override async Task<CurrencyDTOResponse> GetCurrencyOnDate(CurrencyOnDateRequest request, ServerCallContext context)
        {
            CurrencyDTO currencyDTO = await _callerService.GetCurrencyOnDateAsync((CurrencyType)request.CurrencyType,
                (CurrencyType)request.BaseCurrencyType, request.Date.ToDateTime(), context.CancellationToken);
            CurrencyDTOResponse response = new CurrencyDTOResponse()
            {
                CurrencyType = request.CurrencyType,
                Value = (double)currencyDTO.Value
            };
            return response;
        }

        public override async Task<CurrencyDTOResponse> GetCurrentCurrency(CurrentCurrencyRequest request, ServerCallContext context)
        {
            CurrencyDTO currencyDTO = await _callerService.GetCurrentCurrencyAsync((CurrencyType)request.CurrencyType, (CurrencyType)request.BaseCurrencyType, context.CancellationToken);
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
                HasRequests = await _httpService.HasTokens(context.CancellationToken),
            };
            return status;
        }
    }
}
