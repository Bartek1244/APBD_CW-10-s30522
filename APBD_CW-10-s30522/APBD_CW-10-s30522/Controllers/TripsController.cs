using APBD_CW_10_s30522.DTOs;
using APBD_CW_10_s30522.Exceptions;
using APBD_CW_10_s30522.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_CW_10_s30522.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController(IDbService dbService) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetTripsAsync([FromQuery] int page, [FromQuery] int pageSize = 10)
    {
        return Ok(await dbService.GetTripsAsync(page, pageSize));
    }

    //Zmodyfikuje lekko treść polecenia - Jeśli klient o danym peselu już istnieje - to sprawdzam zgodnosc pozostałych danych.
    //Jeśli się nie zgadzają to rzucam błąd, w przeciwnym razie postępuje dalej traktując że client już istnieje. Jeśli
    //client o takim peselu nie istnieje, to go dodaje do clients i postepuje dalej.
    [HttpPost("{idTrip}/clients")]
    public async Task<IActionResult> AssignClientToTripWithOptionalClientCreationAsync(
            [FromBody] ClientAssignmentDTO clientAssignment, [FromRoute] int idTrip)
    {
        try
        {
            var clientTrip = await dbService.AssignClientToTripWithOptionalClientCreationAsync(clientAssignment, idTrip);
            return StatusCode(201, clientTrip);
        }
        catch (ConflictException conflictE)
        {
            return Conflict(conflictE.Message);
        }
        catch (NotFoundException notFoundE)
        {
            return NotFound(notFoundE.Message);
        }
    }
    
}