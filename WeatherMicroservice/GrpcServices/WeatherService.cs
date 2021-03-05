using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Placeme.Infrastructure.Tracing;
using Serilog;
using WeatherMicroservice.Queries;
using WeatherMicroservice.Services;

namespace WeatherMicroservice.GrpcServices
{
    public class WeatherService : Weather.WeatherBase
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly IHttpTraceId traceId;

        public WeatherService(ILogger logger, IMediator mediator, IHttpTraceId traceId)
        {
            _logger = logger;
            _mediator = mediator;
            this.traceId = traceId;
        }

        public override async Task<WeatherReply> GetForecast(Empty request, ServerCallContext context)
        {
            _logger.Information("App: Getting Forecasts via Grpc {TraceId}", this.traceId.GetTraceId());

            var result = await _mediator.Send(new GetForecastQuery());
            var forecasts = result.Select(r => new WeatherForecast
            {
                Date = Timestamp.FromDateTime(r.Date),
                TemperatureC = r.TemperatureC,
                Summary = r.Summary
            }).ToList();

            return new WeatherReply
            {
                Forecasts = {forecasts},
                Count = forecasts.Count
            };
        }

        public override async Task GetForecastStream(Empty request, IServerStreamWriter<WeatherForecast> responseStream,
            ServerCallContext context)
        {
            var result = await _mediator.Send(new GetForecastQuery());
            var forecasts = result.Select(r => new WeatherForecast
            {
                Date = Timestamp.FromDateTime(r.Date),
                TemperatureC = r.TemperatureC,
                Summary = r.Summary
            }).ToList();

            foreach (var forecast in forecasts)
            {
                if (context.CancellationToken.IsCancellationRequested) break;
                _logger.Information("Populating forecast");
                await Task.Delay(500); // Looking busy
                _logger.Information("Sending WeatherData response");
                await responseStream.WriteAsync(forecast);
            }
        }
    }
}