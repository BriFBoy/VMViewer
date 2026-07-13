using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using VMViewer.Controllers;
using VMViewer.Model;
using VMViewer.Service;

namespace WMViewer.Api.Tests.Controllers;


public class PlayerControllerTests
{
    [Test]
    public void AddPlayerTest_WithInvalidTeamId_ReturnsBadRequest()
    {
        var player = new Player(1, "Martin Ødegaard", 25, 0, false);
        var logger = Substitute.For<ILogger<PlayerController>>();
        var playerservice = Substitute.For<IPlayerService>();
        playerservice.AddPlayer(player.Name, player.Age, player.TeamId).Returns((player, ServiceStatus.Invaild));
        var playerController = new PlayerController(playerservice, logger);
        var request = new PlayerController.CreateRequest(player.Age, player.Name, player.TeamId);

        var actionResult = playerController.AddPlayer(request);

        Assert.IsInstanceOf<BadRequestObjectResult>(actionResult);
    }
    
    [Test]
    public void AddPlayerTest_WithValidPlayer_ReturnsCreatedInAction()
    {
        var player = new Player(1, "Martin Ødegaard", 25, 1, false);
        var logger = Substitute.For<ILogger<PlayerController>>();
        var playerservice = Substitute.For<IPlayerService>();
        playerservice.AddPlayer(player.Name, player.Age, player.TeamId).Returns((player, ServiceStatus.Normal));
        var playerController = new PlayerController(playerservice, logger);
        var request = new PlayerController.CreateRequest(player.Age, player.Name, player.TeamId);

        var actionResult = playerController.AddPlayer(request);

        Assert.IsInstanceOf<CreatedAtActionResult>(actionResult);
        Assert.AreEqual(((CreatedAtActionResult)actionResult).Value, player);
    }
    [Test]
    public void AddPlayerTest_WithExistingPlayer_ReturnsConflict()
    {
        var player = new Player(1, "Martin Ødegaard", 25, 1, false);
        var logger = Substitute.For<ILogger<PlayerController>>();
        var playerservice = Substitute.For<IPlayerService>();
        playerservice.AddPlayer(player.Name, player.Age, player.TeamId).Returns((player, ServiceStatus.Exists));
        var playerController = new PlayerController(playerservice, logger);
        var request = new PlayerController.CreateRequest(player.Age, player.Name, player.TeamId);

        var actionResult = playerController.AddPlayer(request);

        Assert.IsInstanceOf<ConflictObjectResult>(actionResult);
    }
    [Test]
    public void AddPlayerTest_WithFullSquad_ReturnsConflict()
    {
        var player = new Player(1, "Martin Ødegaard", 25, 1, false);
        var logger = Substitute.For<ILogger<PlayerController>>();
        var playerservice = Substitute.For<IPlayerService>();
        playerservice.AddPlayer(player.Name, player.Age, player.TeamId).Returns((player, ServiceStatus.TooMany));
        var playerController = new PlayerController(playerservice, logger);
        var request = new PlayerController.CreateRequest(player.Age, player.Name, player.TeamId);

        var actionResult = playerController.AddPlayer(request);

        Assert.IsInstanceOf<ConflictObjectResult>(actionResult);
    }
    
    [Test]
    public void DeletePlayer_WithInvalidId_ReturnsNotFound()
    {
        const int playerId = 1;
        var logger = Substitute.For<ILogger<PlayerController>>();
        var playerservice = Substitute.For<IPlayerService>();
        playerservice.DeletePlayer(playerId).Returns((ServiceStatus.NotFound));
        var playerController = new PlayerController(playerservice, logger);


        var actionResult = playerController.DeletePlayer(playerId);

        Assert.IsInstanceOf<NotFoundResult>(actionResult);
    }
    [Test]
    public void DeletePlayer_WithValidId_ReturnsOk()
    {
        const int playerId = 1;
        var logger = Substitute.For<ILogger<PlayerController>>();
        var playerService = Substitute.For<IPlayerService>();
        playerService.DeletePlayer(playerId).Returns((ServiceStatus.Normal));
        var playerController = new PlayerController(playerService, logger);


        var actionResult = playerController.DeletePlayer(playerId);

        Assert.IsInstanceOf<OkResult>(actionResult);
    }
    [Test]
    public void GetPlayer_WithValidId_ReturnsOk()
    {
        var player = new Player(1, "Martin Ødegaard", 25, 1, false);
        var logger = Substitute.For<ILogger<PlayerController>>();
        var playerService = Substitute.For<IPlayerService>();
        playerService.GetPlayer((int)player.PlayerId!).Returns((player, ServiceStatus.Normal));
        var playerController = new PlayerController(playerService, logger);


        var actionResult = playerController.GetPlayer((int)player.PlayerId!);

        Assert.IsInstanceOf<OkObjectResult>(actionResult);
        Assert.AreEqual(((OkObjectResult)actionResult).Value, player);
    }
    [Test]
    public void GetPlayer_WithInvalidId_ReturnsBadRequest()
    {
        var player = new Player(1, "Martin Ødegaard", 25, 1, false);
        var logger = Substitute.For<ILogger<PlayerController>>();
        var playerService = Substitute.For<IPlayerService>();
        playerService.GetPlayer((int)player.PlayerId!).Returns((player, ServiceStatus.Invaild));
        var playerController = new PlayerController(playerService, logger);


        var actionResult = playerController.GetPlayer((int)player.PlayerId!);

        Assert.IsInstanceOf<BadRequestObjectResult>(actionResult);
    }
}