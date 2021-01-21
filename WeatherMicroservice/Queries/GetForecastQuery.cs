using System.Collections.Generic;
using MediatR;
using WeatherMicroservice.Models;

namespace WeatherMicroservice.Queries
{
    public class GetForecastQuery : IRequest<IEnumerable<WeatherForecastModel>>
    {
    }
}