using System;
using System.Text.Json;
using System.Threading.Tasks;
using Dapr.AppCallback.Autogen.Grpc.v1;
using Dapr.Client.Autogen.Grpc.v1;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Placeme.Infrastructure.Tracing;
using WeatherMicroservice.Queries;
using WeatherMicroservice.Services;

namespace WeatherMicroservice.Dapr
{
    public class DaprGrpcDispatcher : AppCallback.AppCallbackBase
    {
        private readonly JsonSerializerOptions _jsonOptions = new() {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
        private readonly IMediator _mediator;

        public DaprGrpcDispatcher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task<InvokeResponse> OnInvoke(InvokeRequest request, ServerCallContext context)
        {
            var httpContext = context.GetHttpContext();
            var logger = httpContext.RequestServices.GetRequiredService<ILogger<DaprGrpcDispatcher>>();
            var traceId = httpContext.RequestServices.GetRequiredService<IHttpTraceId>();
            logger.LogInformation("App: Getting Forecasts via DaprGrpc {TraceId}", traceId.GetTraceId());

            foreach (var header in httpContext.Request.Headers)
            {
                logger.LogInformation("Header: {Key}: {Value}", header.Key, header.Value.ToString());
            }

            var response = new InvokeResponse();
            switch (request.Method)
            {
                case "GetForecast":
                    var weatherReply = await _mediator.Send(new GetForecastQuery());
                    response.Data = new Any
                    {
                        TypeUrl = WeatherReply.Descriptor.File.Name,
                        Value = ByteString.CopyFrom(JsonSerializer.SerializeToUtf8Bytes(weatherReply, _jsonOptions))
                    };
                    return response;
                default:
                    throw new NotImplementedException($"Requested method {request.Method} not found.");
            }
        }
    }
}