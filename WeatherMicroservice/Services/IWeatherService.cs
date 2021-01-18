using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace WeatherMicroservice.Services
{
    public interface IWeatherService
    {
        public Task<WeatherReply> GetForecast(Empty request, ServerCallContext context);

        public Task<IEnumerable<WeatherForecastDto>> GetForecastDto();
    }
}