using System;
using System.Text.Json;
using System.Threading.Tasks;
using Dapr.AppCallback.Autogen.Grpc.v1;
using Dapr.Client.Autogen.Grpc.v1;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using WeatherMicroservice.Services;

namespace WeatherMicroservice.Dapr
{
    public class DaprGrpcDispatcher : AppCallback.AppCallbackBase
    {
        private readonly JsonSerializerOptions _jsonOptions = new() {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
        private readonly IWeatherService _weatherService;

        public DaprGrpcDispatcher(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        public override async Task<InvokeResponse> OnInvoke(InvokeRequest request, ServerCallContext context)
        {
            var response = new InvokeResponse();
            switch (request.Method)
            {
                case "GetForecast":
                    WeatherReply weatherReply = await _weatherService.GetForecast(new Empty(), context);
                    // var reply = await _weatherService.GetForecastDto();
                    var any = new Any
                    {
                        TypeUrl = WeatherReply.Descriptor.File.Name,
                        Value = weatherReply.ToByteString()
                        //Value = ByteString.CopyFrom(JsonSerializer.SerializeToUtf8Bytes(weatherReply, _jsonOptions))
                        //Value = ByteString.CopyFrom(JsonSerializer.SerializeToUtf8Bytes(reply, _jsonOptions))
                    };
                    response.Data = any;
                    return response;
                default:
                    throw new NotImplementedException($"Requested method {request.Method} not found.");
            }
        }
    }
}