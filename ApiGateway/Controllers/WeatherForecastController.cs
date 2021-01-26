using System.Collections.Generic;
using System.Threading.Tasks;
using ApiGateway.Models;
using ApiGateway.Services;
using Microsoft.AspNetCore.Mvc;
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

        public WeatherForecastController(ILogger logger, IWeatherService weatherService)
        {
            _logger = logger;
            _weatherService = weatherService;
        }

        [HttpGet("dapr-grpc")]
        public async Task<IEnumerable<WeatherForecastDto>> GetByDapr()
        {
            _logger.Information("APIGW: Getting Forecasts via DaprGrpc");
            return await _weatherService.GetForecastsByDaprGrpc();
        }

        [HttpGet("core-grpc")]
        public async Task<WeatherReply> GetByGrpc()
        {
            return await _weatherService.GetForecastsByGrpc();
        }

        [HttpGet("webapi")]
        public async Task<IEnumerable<WeatherForecastDto>> GetWebApi()
        {
            _logger.Information("APIGW: Getting Forecasts via Dapr Rest");
            return await _weatherService.GetForecastsByRest();
        }
    }
}