using NSubstitute;
using NUnit.Framework;
using VMViewer.Model;
using VMViewer.Repository;
using VMViewer.Service;

namespace WMViewer.Api.Tests.Repository;

public class TeamRepositoryTests
{
    private ITeamRepository teamRepository = Substitute.For<ITeamRepository>();
    [SetUp]
    public void SetUp()
    {
        const string name = "";
        teamRepository.SaveTeam(Arg.Any<Team>()).Returns((new Team(1, name), SaveStatus.Created));
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
        var teamService = new TeamService(teamRepository);

     
        
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
        var TeamService = new TeamService(teamRepository);
        

        
        // Act
        var (team, status) = TeamService.AddTeam(name);

        // Assert
        Assert.AreEqual(ServiceStatus.Normal, status);

    }
}