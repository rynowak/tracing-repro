using System.Collections.Generic;
using System.Threading.Tasks;
using ApiGeteway.Models;
using WeatherMicroservice.Services;

namespace ApiGeteway.Services
{
    public interface IWeatherService
    {
        Task<IEnumerable<WeatherForecastDto>> GetForecastsByDapr();
        Task<WeatherReply> GetForecastsByGrpc();
    }
}