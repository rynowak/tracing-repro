using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WeatherMicroservice.Models;

namespace WeatherMicroservice.Repository
{
    public class WeatherForecastRepository : IWeatherForecastRepository
    {
        private readonly ILogger<WeatherForecastRepository> _logger;

        private static readonly string[] Summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public WeatherForecastRepository(ILogger<WeatherForecastRepository> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<WeatherForecastModel>> GetForecastsAsync()
        {
            var rng = new Random();
            var now = DateTime.UtcNow;

            _logger.LogInformation("Generating 100 random forecasts");
            var forecasts = Enumerable.Range(1, 100).Select(i => new WeatherForecastModel
                {
                    Date = now.AddDays(i),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();

            await Task.Delay(2000); // Gotta look busy

            _logger.LogInformation("returning generated forecasts");
            return forecasts;
        }
    }
}