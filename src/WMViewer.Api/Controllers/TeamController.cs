using Microsoft.AspNetCore.Mvc;
using VMViewer.Model;
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

    [Route("addteam")]
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
    [Route("deleteteam/{Id:int}")]
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
    [Route("makecaptain/{teamid:int}/{playerid:int}")]
    public IActionResult MakeCaptain(int teamid, int playerid)
    {
        var (player, status) = teamService.MakeCaptain(teamid, playerid);
        switch (status)
        {
            case ServiceStatus.Normal: return Ok(player);
            default: return Problem();
        }
    }
}

public class TeamRequest
{
    public int? Id { get; set; }
    public string? Name { get; set; }
}