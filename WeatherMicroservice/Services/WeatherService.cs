using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using WeatherMicroservice.Dapr;

namespace WeatherMicroservice.Services
{
    public class WeatherService : Weather.WeatherBase, IWeatherService
    {
        private static readonly string[] Summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherService> _logger;

        public WeatherService(ILogger<WeatherService> logger)
        {
            _logger = logger;
        }

        public override async Task<WeatherReply> GetForecast(Empty request, ServerCallContext context)
        {
            var rng = new Random();
            var now = DateTime.UtcNow;

            var forecasts = Enumerable.Range(1, 100).Select(index => new WeatherForecast
                {
                    Date = Timestamp.FromDateTime(now.AddDays(index)),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();

            await Task.Delay(2000); // Gotta look busy

            return new WeatherReply
            {
                Forecasts = {forecasts},
                Count = forecasts.Length
            };
        }

        public async Task<IEnumerable<WeatherForecastDto>> GetForecast()
        {
            var rng = new Random();
            var now = DateTime.UtcNow;

            var forecasts = Enumerable.Range(1, 100).Select(index => new WeatherForecastDto
                {
                    Date = now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();

            await Task.Delay(2000); // Gotta look busy
            return forecasts;
        }

        public override async Task GetForecastStream(Empty request, IServerStreamWriter<WeatherForecast> responseStream,
            ServerCallContext context)
        {
            var rng = new Random();
            var now = DateTime.UtcNow;
            var i = 0;

            while (!context.CancellationToken.IsCancellationRequested && i < 20)
            {
                _logger.LogInformation("Populating forecast");
                await Task.Delay(500); // Looking busy
                var forecast = new WeatherForecast
                {
                    Date = Timestamp.FromDateTime(now.AddDays(i++)),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                };
                _logger.LogInformation("Sending WeatherData response");

                await responseStream.WriteAsync(forecast);
            }
        }
    }
}