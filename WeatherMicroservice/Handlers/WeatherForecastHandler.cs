using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using WeatherMicroservice.Models;
using WeatherMicroservice.Queries;
using WeatherMicroservice.Repository;

namespace WeatherMicroservice.Handlers
{
    public class WeatherForecastHandler : IRequestHandler<GetForecastQuery, IEnumerable<WeatherForecastModel>>
    {
        private readonly ILogger<WeatherForecastHandler> _logger;
        private readonly IWeatherForecastRepository _weatherForecastRepository;

        public WeatherForecastHandler(IWeatherForecastRepository weatherForecastRepository,
            ILogger<WeatherForecastHandler> logger)
        {
            _weatherForecastRepository = weatherForecastRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<WeatherForecastModel>> Handle(GetForecastQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching weather data from weather repo");
            return await _weatherForecastRepository.GetForecastsAsync();
        }
    }
}