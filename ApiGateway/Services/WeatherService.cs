using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ApiGateway.Models;
using Dapr.Client;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Serilog;
using WeatherMicroservice.Services;

namespace ApiGateway.Services
{
    public class WeatherService : IWeatherService
    {
        private const string WeatherHttp = "weather-http";
        private const string WeatherGrpc = "weather-grpc";
        private readonly DaprClient _dapr;
        private readonly string _daprUrl = $"http://localhost:{DaprHttpPort}/v1.0/invoke/{WeatherHttp}";
        private readonly ILogger _logger;

        public WeatherService(ILogger logger, DaprClient dapr)
        {
            _logger = logger;
            _dapr = dapr;
        }

        private static string DaprHttpPort => Environment.GetEnvironmentVariable("DAPR_PORT") ?? "3500";

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
            var httpClient = new HttpClient();
            var url = $"{_daprUrl}/method/Weather";
            _logger.Information("Executing Rest call via Dapr {Url}", url);
            try
            {
                var streamAsync = httpClient.GetStreamAsync(url);
                return await JsonSerializer.DeserializeAsync<IEnumerable<WeatherForecastDto>>(await streamAsync);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error fetching forecast data via Dapr");
            }

            return new List<WeatherForecastDto>();
        }

        public async Task<IEnumerable<WeatherForecastDto>> GetForecastsByDaprGrpc()
        {
            Console.WriteLine("Invoking grpc weather forecasts");

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