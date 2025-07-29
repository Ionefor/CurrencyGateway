using System.Threading.Tasks;
using CurrencyGateway.Application.Handlers;
using CurrencyGateway.Application.Requests;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyGateway.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetCurrencyRates(
            [FromQuery] GetCurrencyRequest request,
            [FromServices] GetCurrencyHandler handler)
        {
           var result = await handler.Handle(request);
           
           if(result.IsFailure)
               return NoContent();
           
           return Ok(result.Value);
        }
    }
}