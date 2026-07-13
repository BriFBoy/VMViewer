using Microsoft.AspNetCore.Mvc;
using VMViewer.Repository;
using VMViewer.Service;

namespace VMViewer.Controllers;

[ApiController]
[Route("squad")]
public class SquadController(ISquadService squadService, ILogger<SquadController> logger) : ControllerBase
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
        switch (status)
        {
            case ServiceStatus.Normal:
                logger.LogInformation("Transfered player with id {playerid} to team with id {teamid}", playerid, teamid);
                return Ok(player);
            case ServiceStatus.NotFound:
                return NoContent();
            case ServiceStatus.Invaild:
                return BadRequest("Team doesnt exist");
            default: return Problem();
        }
    }
}