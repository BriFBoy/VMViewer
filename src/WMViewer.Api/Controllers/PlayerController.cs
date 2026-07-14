using Microsoft.AspNetCore.Mvc;
using VMViewer.Service;

namespace VMViewer.Controllers;

[ApiController]
[Route("player")]
public class PlayerController(IPlayerService playerService, ILogger<PlayerController> logger) : ControllerBase
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
    [Route("")]
    public IActionResult AddPlayer([FromBody] CreateRequest request)
    {
        var (player, status) = playerService.AddPlayer(request.Name, request.Age, request.TeamId);
        switch (status)
        {
            case ServiceStatus.Normal:
                logger.LogInformation("Created new player with id {id}", player.PlayerId);
                return CreatedAtAction(nameof(AddPlayer), player);
            case ServiceStatus.Invaild:
                return BadRequest("Team does not exists");
            case ServiceStatus.Exists:
                return Conflict("Player already exists");
            case ServiceStatus.TooMany:
                return Conflict("Too many players in squad");
            default:
                return Problem();
        }
    }

    [HttpDelete]
    [Route("{Id:int}")]
    public IActionResult DeletePlayer(int Id)
    {
        var res = playerService.DeletePlayer(Id);

         switch (res)
        {
            case ServiceStatus.Normal:
                logger.LogInformation("Deleted player with id " + Id);
                return Ok();
            case ServiceStatus.NotFound:
                return NotFound();
            default:
                return Problem();
        }
    }

    public class CreateRequest(int age, string name, int teamId)
    {
        public int Age { get; } = age;
        public string Name { get; } = name;
        public int TeamId { get; } = teamId;
    }
}