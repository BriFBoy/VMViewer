using Microsoft.AspNetCore.Mvc;
using VMViewer.Repository;
using VMViewer.Service;

namespace VMViewer.Controllers;

[ApiController]
[Route("squad")]
public class SquadController(ISquadService squadService): ControllerBase
{
    [HttpGet]
    [Route("{Id:int}")]
    public IActionResult GetSquad(int id)
    {
        var (sqaud, status) = squadService.GetSquad(id);
        return status switch
        {
            ServiceStatus.Normal => Ok(sqaud),
            ServiceStatus.NotFound => NoContent(),
            _ => Problem()
        };
    }
    
}