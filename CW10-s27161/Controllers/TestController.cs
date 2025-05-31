using CW10_s27161.Data;
using CW10_s27161.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CW10_s27161.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController(DbService context) : ControllerBase
    {
        
        
        
        /*[HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await context.Trips.Select(t => new {id = t.IdTrip}).ToListAsync());
        }*/

        [HttpGet("/trips")]
        public async Task<ActionResult> GetTrips()
        {
            var trips = context.GetTripsAsync();
            return Ok(trips);
        }
    }
}