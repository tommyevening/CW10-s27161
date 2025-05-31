using Microsoft.AspNetCore.Mvc;

namespace CW10_s27161.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class ClientsController(IDbService dbService) : ControllerBase
    {
        [HttpDelete("{idClient}")]
        public async Task<IActionResult> DeleteClient(int idClient)
        {
            var client = await dbService.GetClientAsync(idClient);
            if (client == null)
            {
                return NotFound();                
            }

            var result = await dbService.DeleteClient(idClient);
            return result;
        }
    }
}