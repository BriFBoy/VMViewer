using Microsoft.AspNetCore.Mvc;
using VMViewer.Service;

namespace VMViewer.Controllers;

[ApiController]
[Route("player")]
public class PlayerController(IPlayerService playerService) : ControllerBase
{
  [HttpGet]
  [Route("{id:int}")]
  public IActionResult GetPlayer(int id)
  {
    var (player, status) = playerService.GetPlayer(id);

    return status switch
    {

      ServiceStatus.NotFound => NoContent(),
      ServiceStatus.Normal => Ok(player),
      ServiceStatus.Invaild => BadRequest("Invalid ID"),
      _ => Problem()
    };




  }
  
  [HttpPost]
  [Route("addplayer")]
  public IActionResult AddPlayer([FromBody] CreateRequest request)
  {
    var (player, status) = playerService.AddPlayer(request.Name, request.Age, request.TeamId);
    return status switch
    {
      ServiceStatus.Normal => CreatedAtAction( nameof(AddPlayer), player),
      ServiceStatus.Invaild => BadRequest("Team does not exists"),
      ServiceStatus.Exists => Conflict("Player already exists"),
      ServiceStatus.TooMany => Conflict("Too many players in squad"),
      _ => Problem(),
    };
  }

  [HttpDelete]
  [Route("deleteplayer/{Id:int}")]
  public IActionResult DeletePlayer(int Id)
  {
    var res = playerService.DeletePlayer(Id);

    return res switch
    {
      ServiceStatus.Normal => Ok(),
      ServiceStatus.NotFound => NotFound(),
      _ => Problem()
    };
  }

  public class CreateRequest(int age, string name, int teamId)
  {

    public int Age { get; } = age;
    public string Name { get; } = name;
    public int TeamId { get; } = teamId;

  }
}

