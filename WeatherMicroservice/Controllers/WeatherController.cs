using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WeatherMicroservice.Queries;

namespace WeatherMicroservice.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WeatherController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET
        [HttpGet]
        public async Task<ObjectResult> Index()
        {
            var result = await _mediator.Send(new GetForecastQuery());
            return Ok(result);
        }
    }
}