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
    [Route("transfer")]
    public IActionResult TransferPlayer([FromBody] Transfer requset)
    {
        var (player, status) = squadService.Transfer(requset.PlayerId, requset.TeamId);
        switch (status)
        {
            case ServiceStatus.Normal:
                logger.LogInformation("Transfered player with id {playerid} to team with id {teamid}", requset.PlayerId, requset.TeamId);
                return Ok(player);
            case ServiceStatus.NotFound:
                return NoContent();
            case ServiceStatus.Invaild:
                return BadRequest("Team doesnt exist");
            default: return Problem();
        }
    }
}

public class Transfer(int playerId, int teamId)
{
    public readonly int PlayerId = playerId;
    public readonly int TeamId = teamId;
}