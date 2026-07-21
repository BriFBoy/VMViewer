using Microsoft.AspNetCore.Mvc;
using Shared.Model;
using VMViewer.Service;

namespace VMViewer.Controllers;

[ApiController]
[Route("team")]
public class TeamController(ITeamService teamService, ILogger<TeamController> logger) : ControllerBase
{
    [Route("{id:int}")]
    public ActionResult<Team> GetTeam(int Id)
    {
        var team = teamService.GetTeam(Id);
        return team is not null ? Ok(team) : NoContent();
    }

    [Route("")]
    [HttpPost]
    public ActionResult<Team> AddTeam(TeamRequest request)
    {
        var (team, status) = teamService.AddTeam(request.Name);
        switch (status)
        {
            case ServiceStatus.Exists:
                return Conflict("Already exists");
            case ServiceStatus.Normal:
                logger.LogInformation("Created Team with id {teamid}", team.TeamId);
                return CreatedAtAction("addteam", team);
            default:
                return Problem();
        }
    }

    [HttpDelete]
    [Route("{Id:int}")]
    public IActionResult DeleteTeam(int Id)
    {
        switch (teamService.DeleteTeam(Id))
        {
            case ServiceStatus.NotFound:
                return NoContent();
            case ServiceStatus.Normal:
                logger.LogInformation("Deleted Team with id {teamid}", Id);
                return Ok();
            default: return Problem();
        }
    }

    [HttpPut]
    [Route("captain")]
    public IActionResult MakeCaptain([FromBody] CaptainRequest request)
    {
        var (player, status) = teamService.MakeCaptain(request.TeamId, request.PlayerId);
        return status switch
        {
            ServiceStatus.Normal => Ok(player),
            _ => Problem()
        };
    }
}

public class TeamRequest
{
    public int? Id { get; set; }
    public string? Name { get; set; }
}

public class CaptainRequest(int playerId, int teamId)
{
    public int TeamId { get; } = teamId;
    public int PlayerId { get;  } = playerId;
}