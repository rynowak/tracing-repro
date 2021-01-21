using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WeatherMicroservice.Models;
using WeatherMicroservice.Queries;
using WeatherMicroservice.Repository;

namespace WeatherMicroservice.Handlers
{
    public class WeatherForecastHandler : IRequestHandler<GetForecastQuery, IEnumerable<WeatherForecastModel>>
    {
        private readonly IWeatherForecastRepository _weatherForecastRepository;

        public WeatherForecastHandler(IWeatherForecastRepository weatherForecastRepository)
        {
            _weatherForecastRepository = weatherForecastRepository;
        }

        public async Task<IEnumerable<WeatherForecastModel>> Handle(GetForecastQuery request,
            CancellationToken cancellationToken)
        {
            return await _weatherForecastRepository.GetForecastsAsync();
        }
    }
}