using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ApiGeteway.Models;
using Dapr.Client;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using WeatherMicroservice.Services;

namespace ApiGeteway.Services
{
    public class WeatherService : IWeatherService
    {
        private const string WeatherName = "weather";
        private const int DaprPort = 3500;
        private readonly DaprClient _dapr;
        private readonly string _daprUrl = $"http://localhost:${DaprPort}/v1.0/invoke/${WeatherName}";
        private ILogger<WeatherService> _logger;

        public WeatherService(ILoggerFactory loggerFactory, Weather.WeatherClient client, DaprClient dapr)
        {
            _logger = loggerFactory.CreateLogger<WeatherService>();
            _dapr = dapr;
        }

        public async Task<IEnumerable<WeatherForecastDto>> GetForecasts()
        {
            await InvokeGrpcBalanceServiceOperationAsync();

            var channel = GrpcChannel.ForAddress(_daprUrl);
            var client = new Weather.WeatherClient(channel);
            var response = await client.GetForecastAsync(new Empty());

            return response.Forecasts.Select(r => new WeatherForecastDto
            {
                Date = r.Date.ToDateTime(),
                Summary = r.Summary,
                TemperatureC = r.TemperatureC
            });
        }

        internal async Task<IEnumerable<WeatherForecastDto>> InvokeGrpcBalanceServiceOperationAsync()
        {
            Console.WriteLine("Invoking grpc weather forecasts");

            try
            {
                var res = await _dapr.InvokeMethodAsync<IEnumerable<WeatherForecastDto>>(WeatherName, "GetForecast");
                Console.WriteLine($"Mir hei {res.Count()} vorhärsage:");
                Console.WriteLine($"{JsonSerializer.Serialize(res)}");
                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }
    }

    public interface IWeatherService
    {
        Task<IEnumerable<WeatherForecastDto>> GetForecasts();
    }
}