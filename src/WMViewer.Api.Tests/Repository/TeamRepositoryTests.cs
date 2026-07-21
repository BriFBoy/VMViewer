using NSubstitute;
using NUnit.Framework;
using Shared.Model;
using VMViewer.Repository;
using VMViewer.Service;

namespace WMViewer.Api.Tests.Repository;

public class TeamRepositoryTests
{
    private ITeamRepository teamRepository = Substitute.For<ITeamRepository>();
    private IPlayerRepository _playerRepository = Substitute.For<IPlayerRepository>();
    [SetUp]
    public void SetUp()
    {
        const string name = "";
        teamRepository.SaveTeam(Arg.Any<Team>()).Returns((new Team(1, name, 0, null), SaveStatus.Created));
    }

    [TearDown]
    public void TearDown()
    {
        Console.WriteLine("bye");
    }

   
    [Test]
    public void AddTeam_WithInvalidName_ReturnsInvalid()
    {
        const string name = "";
        var teamService = new TeamService(teamRepository, _playerRepository);

     
        
        // Act
        var (team, status) = teamService.AddTeam(name);

        // Assert
        Assert.AreEqual(ServiceStatus.Invaild, status);
        Assert.AreEqual(null, team);
    }
    
    [Test]
    public void AddTeam_WithVaildName_ReturnsNormal()
    {
        var name = "Norway";
        var TeamService = new TeamService(teamRepository, _playerRepository);
        

        
        // Act
        var (team, status) = TeamService.AddTeam(name);

        // Assert
        Assert.AreEqual(ServiceStatus.Normal, status);

    }
}