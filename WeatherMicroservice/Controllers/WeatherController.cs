using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Placeme.Infrastructure.Tracing;
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
            var logger = HttpContext.RequestServices.GetRequiredService<ILogger<WeatherController>>();
            var traceId = HttpContext.RequestServices.GetRequiredService<IHttpTraceId>();
            logger.LogInformation("App: Getting Forecasts via REST {TraceId}", traceId.GetTraceId());

            var result = await _mediator.Send(new GetForecastQuery());
            return Ok(result);
        }
    }
}