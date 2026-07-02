using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using VMViewer.Model;
using VMViewer.Repository;
using VMViewer.Service;

namespace VMViewer.Controllers;

[ApiController]
[Route("team")]
public class TeamController(ITeamService teamService) : ControllerBase
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
    return status switch
    {
      ServiceStatus.Exists => Conflict("Already exists"),
      ServiceStatus.Normal => CreatedAtAction("addteam", team),
      _ => Problem()
    };
  }
  
  [HttpDelete]
  [Route("deleteteam/{Id:int}")]
  public IActionResult DeleteTeam(int Id)
  {
    return teamService.DeleteTeam(Id) switch
    {
      ServiceStatus.NotFound => NoContent(),
      ServiceStatus.Normal => Ok(),
      _ => Problem()
    };
  }


}

public class TeamRequest
{
  public int? Id { get; set; }
  public string? Name { get; set; }

}
