using CW10_s27161.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CW10_s27161.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TripsController(IDbService dbService) : ControllerBase 
    {
        
        
        
        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await dbService.GetTripsAsync(page, pageSize);
            return Ok(result);
        }

        [HttpPost("{idTrip}/clients")]
        public async Task<ActionResult> AddClientToTrip(int idTrip, [FromBody] CreateClientTripDTO dto)
        {
            dto.IdTrip = idTrip;
            return Ok(await dbService.AddClientToTrip(dto));
        }
    }
}