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
    [HttpPut]
    [Route("transfer/{playerid:int}/{teamid:int}")]
    public IActionResult TransferPlayer(int playerid, int teamid)
    {
        var (player, status) = squadService.Transfer(playerid, teamid);
        return status switch
        {
            ServiceStatus.Normal => Ok(player),
            ServiceStatus.NotFound => NoContent(),
            ServiceStatus.Invaild => BadRequest("Team doesnt exist"),
            _ => Problem()
        };
    }
    
}