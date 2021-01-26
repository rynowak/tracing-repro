using System.Collections.Generic;
using System.Threading.Tasks;
using ApiGateway.Models;
using WeatherMicroservice.Services;

namespace ApiGateway.Services
{
    public interface IWeatherService
    {
        Task<IEnumerable<WeatherForecastDto>> GetForecastsByDaprGrpc();
        Task<WeatherReply> GetForecastsByGrpc();
        Task<IEnumerable<WeatherForecastDto>> GetForecastsByRest();
    }
}