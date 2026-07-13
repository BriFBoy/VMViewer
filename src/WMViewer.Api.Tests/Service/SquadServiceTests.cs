using NSubstitute;
using NUnit.Framework;
using VMViewer.Model;
using VMViewer.Repository;
using VMViewer.Service;

namespace WMViewer.Api.Tests.Service;

public class SquadServiceTests
{
    

    
    [Test]
    public void Transfer_WithInvalidTeam_ReturnsInvalid()
    {
        var teamRepository = Substitute.For<ITeamRepository>();
        teamRepository.DoTeamExistsWithId(Arg.Any<int>()).Returns(false);
        var playerRepository = Substitute.For<IPlayerRepository>();
        var squadService = new SquadService(playerRepository, teamRepository);

        var (player, status) = squadService.Transfer(1, 2);
        
        Assert.AreEqual(ServiceStatus.Invaild, status);
        Assert.AreEqual(null, player);


    }
    [Test]
    public void Transfer_WithMaxSquad_ReturnsTooMany()
    {
        var teamRepository = Substitute.For<ITeamRepository>();
        teamRepository.DoTeamExistsWithId(Arg.Any<int>()).Returns(true);
        teamRepository.GetByID(Arg.Any<int>()).Returns(new Team(2, "Norway", 25, null));
        var playerRepository = Substitute.For<IPlayerRepository>();
        var squadService = new SquadService(playerRepository, teamRepository);

        var (player, status) = squadService.Transfer(1, 2);
        
        Assert.AreEqual(ServiceStatus.TooMany, status);
        Assert.AreEqual(null, player);
    }
    [Test]
    public void Transfer_AddsSquadCount()
    {
        var teamRepository = Substitute.For<ITeamRepository>();
        teamRepository.DoTeamExistsWithId(Arg.Any<int>()).Returns(true);
        teamRepository.GetByID(Arg.Any<int>()).Returns(new Team(2, "Norway", 25, null));
        var playerRepository = Substitute.For<IPlayerRepository>();
        var squadService = new SquadService(playerRepository, teamRepository);

        var (player, status) = squadService.Transfer(1, 2);
        
        Assert.AreEqual(ServiceStatus.TooMany, status);
        Assert.AreEqual(null, player);
    }
}