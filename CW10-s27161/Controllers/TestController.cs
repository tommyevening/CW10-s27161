using CW10_s27161.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CW10_s27161.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController(Cw10Context context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await context.Trips.Select(t => new {id = t.IdTrip}).ToListAsync());
        }
    }
}