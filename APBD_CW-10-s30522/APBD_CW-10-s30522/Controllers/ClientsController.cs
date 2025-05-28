using APBD_CW_10_s30522.Exceptions;
using APBD_CW_10_s30522.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_CW_10_s30522.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController(IDbService dbService) : ControllerBase
{
    [HttpDelete("{idClient}")]
    public async Task<IActionResult> DeleteClientAsync([FromRoute] int idClient)
    {
        try
        {
            await dbService.DeleteClientAsync(idClient);
            return NoContent();
        }
        catch (NotFoundException notFoundE)
        {
            return NotFound(notFoundE.Message);
        }
        catch (ConflictException conflictE)
        {
            return Conflict(conflictE.Message);
        }
    }
}