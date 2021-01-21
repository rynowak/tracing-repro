using System.Collections.Generic;
using System.Threading.Tasks;
using WeatherMicroservice.Models;

namespace WeatherMicroservice.Repository
{
    public interface IWeatherForecastRepository
    {
        Task<IEnumerable<WeatherForecastModel>> GetForecastsAsync();
    }
}