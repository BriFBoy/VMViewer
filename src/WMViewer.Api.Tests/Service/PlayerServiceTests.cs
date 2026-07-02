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
        var player = new Player(1, "Name", 22, 1);
        var playerRepository = Substitute.For<IPlayerRepository>();
        playerRepository.AddPlayer(Arg.Any<Player>()).Returns((player, SaveStatus.Created));
        var playerSerivce = new PlayerService(playerRepository);

        var (retplayer, status) = playerSerivce.AddPlayer(player.Name, player.Age, player.TeamId);
        
        Assert.AreEqual(ServiceStatus.Normal, status);
        Assert.AreEqual(player, retplayer);
    }
    [Test]
    public void AddPlayer_WithDataBaseFail_ReturnsError()
    {
        var player = new Player(1, "Name", 22, 1);
        var playerRepository = Substitute.For<IPlayerRepository>();
        playerRepository.AddPlayer(Arg.Any<Player>()).Returns((null, SaveStatus.ErrorOccured));
        var playerSerivce = new PlayerService(playerRepository);

        var (retplayer, status) = playerSerivce.AddPlayer(player.Name, player.Age, player.TeamId);
        
        Assert.AreEqual(ServiceStatus.Error, status);
        Assert.AreEqual(null, retplayer);
    }
}