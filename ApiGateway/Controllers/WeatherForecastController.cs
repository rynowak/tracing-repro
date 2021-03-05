using System.Collections.Generic;
using System.Threading.Tasks;
using ApiGateway.Models;
using ApiGateway.Services;
using Microsoft.AspNetCore.Mvc;
using Placeme.Infrastructure.Tracing;
using Serilog;
using WeatherMicroservice.Services;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IWeatherService _weatherService;
        private readonly IHttpTraceId traceId;

        public WeatherForecastController(ILogger logger, IWeatherService weatherService, IHttpTraceId traceId)
        {
            _logger = logger;
            _weatherService = weatherService;
            this.traceId = traceId;
        }

        [HttpGet("dapr-grpc")]
        public async Task<IEnumerable<WeatherForecastDto>> GetByDapr()
        {
            _logger.Information("APIGW: Getting Forecasts via DaprGrpc {TraceId}", this.traceId.GetTraceId());
            return await _weatherService.GetForecastsByDaprGrpc();
        }

        [HttpGet("core-grpc")]
        public async Task<WeatherReply> GetByGrpc()
        {
            _logger.Information("APIGW: Getting Forecasts via Vanilla Grpc {TraceId}", this.traceId.GetTraceId());
            return await _weatherService.GetForecastsByGrpc();
        }

        [HttpGet("webapi")]
        public async Task<IEnumerable<WeatherForecastDto>> GetWebApi()
        {
            var t = this.traceId.GetTraceId();
            _logger.Information("APIGW: Getting Forecasts via Dapr Rest {TraceId}", t);
            return await _weatherService.GetForecastsByRest();
        }
    }
}