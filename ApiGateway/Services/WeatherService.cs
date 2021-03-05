using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ApiGateway.Models;
using Dapr.Client;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Placeme.Infrastructure.Tracing;
using Serilog;
using WeatherMicroservice.Services;

namespace ApiGateway.Services
{
    public class WeatherService : IWeatherService
    {
        private const string WeatherHttp = "weather-http";
        private const string WeatherGrpc = "weather-grpc";
        private readonly DaprClient _dapr;
        private readonly IHttpTraceId _httpTraceId;
        private readonly ILogger _logger;

        public WeatherService(ILogger logger, DaprClient dapr, IHttpTraceId httpTraceId)
        {
            _logger = logger;
            _dapr = dapr;
            _httpTraceId = httpTraceId;
        }

        private static string WeatherServiceGrpcPort => Environment.GetEnvironmentVariable("WEATHER_GRPC_PORT") ?? "5001";

        public async Task<WeatherReply> GetForecastsByGrpc()
        {
            _logger.Information("Executing vanilla Grpc call {GrpcUrl}", $"http://localhost:{WeatherServiceGrpcPort}");
            var channel = GrpcChannel.ForAddress($"http://localhost:{WeatherServiceGrpcPort}");
            var client = new Weather.WeatherClient(channel);
            var response = await client.GetForecastAsync(new Empty());

            return response;
        }

        public async Task<IEnumerable<WeatherForecastDto>> GetForecastsByRest()
        {
            _logger.Information("Executing Rest call via Dapr to {DaprService}", WeatherHttp);
            try
            {
                return await _dapr.InvokeMethodAsync<IEnumerable<WeatherForecastDto>>(WeatherHttp, "Weather",
                    new HttpInvocationOptions
                    {
                        Method = HttpMethod.Get
                    });
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error fetching forecast data via Dapr");
            }

            return new List<WeatherForecastDto>();
        }

        public async Task<IEnumerable<WeatherForecastDto>> GetForecastsByDaprGrpc()
        {
            try
            {
                _logger.Information("Executing Grpc call via Dapr to {WeatherGrpc}", WeatherGrpc);

                var res = await _dapr.InvokeMethodAsync<IEnumerable<WeatherForecastDto>>(WeatherGrpc, "GetForecast");
                return res;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error fetching forecast data via DaprGrpc");
            }

            return new List<WeatherForecastDto>();
        }
    }
}