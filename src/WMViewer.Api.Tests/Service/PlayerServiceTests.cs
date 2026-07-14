
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using VMViewer.Model;
using VMViewer.Repository;
using VMViewer.Service;

namespace WMViewer.Api.Tests.Service;

public class PlayerServiceTests
{
    [Test]
    public void AddPlayer_withValidPlayer_ReturnsCreated()
    {
        var player = new Player(1, "Name", 22, 1, false);
        var logger = Substitute.For<ILogger<PlayerService>>();
        var playerRepository = Substitute.For<IPlayerRepository>();
        var teamRepository = Substitute.For<ITeamRepository>();
        playerRepository.AddPlayer(Arg.Any<Player>()).Returns((player, SaveStatus.Created));
        teamRepository.DoTeamExistsWithId(Arg.Any<int>()).Returns(true);
        playerRepository.GetAllPlayersInSquad(Arg.Any<int>()).Returns((new List<Player>(12), SaveStatus.Normal));
        var playerSerivce = new PlayerService(playerRepository, teamRepository, logger);

        var (retplayer, status) = playerSerivce.AddPlayer(player.Name, player.Age, player.TeamId);
        
        Assert.AreEqual(ServiceStatus.Normal, status);
        Assert.AreEqual(player, retplayer);
    }
    [Test]
    public void AddPlayer_WithDataBaseFail_ReturnsError()
    {
        var player = new Player(1, "Name", 22, 1, false);
        var logger = Substitute.For<ILogger<PlayerService>>();
        var playerRepository = Substitute.For<IPlayerRepository>();
        var teamRepository = Substitute.For<ITeamRepository>();
        playerRepository.AddPlayer(Arg.Any<Player>()).Returns((null, SaveStatus.ErrorOccured));
        teamRepository.DoTeamExistsWithId(Arg.Any<int>()).Returns(true);
        var playerSerivce = new PlayerService(playerRepository, teamRepository, logger);

        var (retplayer, status) = playerSerivce.AddPlayer(player.Name, player.Age, player.TeamId);
        
        Assert.AreEqual(ServiceStatus.Error, status);
        Assert.AreEqual(null, retplayer);
    }
    [Test]
    public void AddPlayer_WithFullSquad_ReturnsTooMany()
    {
        var player = new Player(1, "Name", 22, 1, false);
        var logger = Substitute.For<ILogger<PlayerService>>();
        var playerRepository = Substitute.For<IPlayerRepository>();
        var teamRepository = Substitute.For<ITeamRepository>();
        playerRepository.AddPlayer(Arg.Any<Player>()).Returns((player, SaveStatus.Normal));
        playerRepository.GetAllPlayersInSquad(Arg.Any<int>()).Returns((new List<Player>(25), SaveStatus.Normal));
        teamRepository.DoTeamExistsWithId(Arg.Any<int>()).Returns(true);
        
        var playerService = new PlayerService(playerRepository, teamRepository, logger);

        var (retplayer, status) = playerService.AddPlayer(player.Name, player.Age, player.TeamId);
        
        Assert.AreEqual(ServiceStatus.Error, status);
        Assert.AreEqual(null, retplayer);
    }
    [Test]
    public void AddPlayer_WithInvalidTeam_ReturnsInvalid()
    {
        var player = new Player(1, "Name", 22, 1, false);
        var logger = Substitute.For<ILogger<PlayerService>>();
        var playerRepository = Substitute.For<IPlayerRepository>();
        var teamRepository = Substitute.For<ITeamRepository>();
        playerRepository.AddPlayer(Arg.Any<Player>()).Returns((player, SaveStatus.Normal));
        teamRepository.DoTeamExistsWithId(Arg.Any<int>()).Returns(false);
        

        var playerService = new PlayerService(playerRepository, teamRepository, logger);

        var (retplayer, status) = playerService.AddPlayer(player.Name, player.Age, player.TeamId);
        
        Assert.AreEqual(ServiceStatus.Invaild, status);
        Assert.AreEqual(null, retplayer);
    }
}