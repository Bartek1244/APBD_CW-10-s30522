using APBD_CW_10_s30522.DTOs;
using APBD_CW_10_s30522.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_CW_10_s30522.Controllers;

[ApiController]
[Route("[controller]")]
public class TripController(IDbService dbService) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult>
        GetTripsAsync([FromQuery] int page, [FromQuery] int pageSize = 10)
    {
        return Ok(await dbService.GetTripsAsync(page, pageSize));
    }
    
}