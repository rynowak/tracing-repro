using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using WeatherMicroservice.Services;

namespace WeatherMicroservice.Dapr
{
    public interface IWeatherService
    {
        public Task<WeatherReply> GetForecast(Empty request, ServerCallContext context);

        public Task<IEnumerable<WeatherForecastDto>> GetForecast();
    }
}