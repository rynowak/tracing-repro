using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ApiGateway.Models;
using Dapr.Client;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using WeatherMicroservice.Services;

namespace ApiGateway.Services
{
    public class WeatherService : IWeatherService
    {
        private const string WeatherHttp = "weather-http";
        private const string WeatherGrpc = "weather-grpc";
        private readonly DaprClient _dapr;
        private readonly string _daprUrl = $"http://localhost:{DaprHttpPort}/v1.0/invoke/{WeatherHttp}";
        private readonly ILogger<WeatherService> _logger;

        public WeatherService(ILoggerFactory loggerFactory, DaprClient dapr)
        {
            _logger = loggerFactory.CreateLogger<WeatherService>();
            _dapr = dapr;
        }

        private static string DaprHttpPort => Environment.GetEnvironmentVariable("DAPR_PORT") ?? "3500";

        private static string WeatherServiceGrpcPort =>
            Environment.GetEnvironmentVariable("WEATHER_GRPC_PORT") ?? "5001";

        public async Task<WeatherReply> GetForecastsByGrpc()
        {
            _logger.LogInformation($"Executing vanilla Grpc call to http://localhost:{WeatherServiceGrpcPort}");
            var channel = GrpcChannel.ForAddress($"http://localhost:{WeatherServiceGrpcPort}");
            var client = new Weather.WeatherClient(channel);
            var response = await client.GetForecastAsync(new Empty());

            return response;
        }

        public async Task<IEnumerable<WeatherForecastDto>> GetForecastsByRest()
        {
            var httpClient = new HttpClient();
            var url = $"{_daprUrl}/method/Weather";
            _logger.LogInformation($"Executing Rest call via Dapr to {url}");
            var streamAsync = httpClient.GetStreamAsync(url);
            var result = await JsonSerializer.DeserializeAsync<IEnumerable<WeatherForecastDto>>(await streamAsync);

            return result;
        }

        public async Task<IEnumerable<WeatherForecastDto>> GetForecastsByDaprGrpc()
        {
            Console.WriteLine("Invoking grpc weather forecasts");

            try
            {
                _logger.LogInformation($"Executing Grpc call via Dapr to {WeatherGrpc}");
                var res = await _dapr.InvokeMethodAsync<IEnumerable<WeatherForecastDto>>(WeatherGrpc, "GetForecast");
                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }
    }
}