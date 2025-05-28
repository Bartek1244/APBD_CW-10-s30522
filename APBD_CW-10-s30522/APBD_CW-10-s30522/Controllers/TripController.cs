using APBD_CW_10_s30522.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_CW_10_s30522.Controllers;

[ApiController]
[Route("[controller]")]
public class TripController(IDbService dbService) : ControllerBase
{
    
}