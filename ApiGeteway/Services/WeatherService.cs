using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiGeteway.Models;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using WeatherMicroservice.Services;

namespace ApiGeteway.Services
{
    public class WeatherService : IWeatherService
    {
        public async Task<IEnumerable<WeatherForecastDto>> GetForecasts()
        {
            var channel = GrpcChannel.ForAddress("http://localhost:5001");
            var client = new Weather.WeatherClient(channel);

            var response = await client.GetForecastAsync(new Empty());
            return response.Forecasts.Select(r => new WeatherForecastDto
            {
                Date = r.Date.ToDateTime(),
                Summary = r.Summary,
                TemperatureC = r.TemperatureC
            });
        }
    }

    public interface IWeatherService
    {
        Task<IEnumerable<WeatherForecastDto>> GetForecasts();
    }
}