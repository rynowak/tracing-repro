using System;
using System.Buffers;
using System.Collections.Generic;
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
        private const int DaprHttpPort = 3500;
        private const int DaprGrpcPort = 3501;
        private readonly DaprClient _dapr;
        private readonly string _daprGrpc = $"http://localhost:${DaprGrpcPort}/v1.0/invoke/${WeatherName}";
        private readonly string _daprGrpc2 = $"http://localhost:${DaprGrpcPort}";
        private readonly string _daprUrl = $"http://localhost:${DaprHttpPort}/v1.0/invoke/${WeatherName}";
        private ILogger<WeatherService> _logger;

        public WeatherService(ILoggerFactory loggerFactory, Weather.WeatherClient client, DaprClient dapr)
        {
            _logger = loggerFactory.CreateLogger<WeatherService>();
            _dapr = dapr;
        }

        public async Task<WeatherReply> GetForecastsByGrpc()
        {
            var channel = GrpcChannel.ForAddress("http://localhost:5001");
            var client = new Weather.WeatherClient(channel);
            var response = await client.GetForecastAsync(new Empty());

            return response;
        }

        public async Task<IEnumerable<WeatherForecastDto>> GetForecastsByDapr()
        {
            Console.WriteLine("Invoking grpc weather forecasts");

            try
            {
                //var res = await _dapr.InvokeMethodAsync<IEnumerable<WeatherForecastDto>>(WeatherName, "GetForecast");
                var res3 = await _dapr.InvokeMethodAsync<object>(WeatherName, "GetForecast");
                var res2 = await _dapr.InvokeMethodAsync<WeatherReply>(WeatherName, "GetForecast");
                /*Console.WriteLine($"Mir hei {res.Count()} vorhärsage:");
                Console.WriteLine($"{JsonSerializer.Serialize(res)}");
                */
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        public static T ToObject<T>(JsonElement element, JsonSerializerOptions options = null)
        {
            var bufferWriter = new ArrayBufferWriter<byte>();
            using (var writer = new Utf8JsonWriter(bufferWriter))
            {
                element.WriteTo(writer);
            }

            return JsonSerializer.Deserialize<T>(bufferWriter.WrittenSpan, options);
        }
    }
}