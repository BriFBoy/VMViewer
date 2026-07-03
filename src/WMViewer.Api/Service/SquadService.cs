using VMViewer.Model;
using VMViewer.Repository;

namespace VMViewer.Service;

public class SquadService(IPlayerRepository playerRepository, ITeamRepository teamRepository): ISquadService
{
    public (List<Player>?, ServiceStatus) GetSquad(int teamid)
    {
        var (squad, status) = playerRepository.GetAllPlayersInSquad(teamid);
        return status switch
        {
            SaveStatus.NoEntries => (null, ServiceStatus.NotFound),
            SaveStatus.Normal => (squad, ServiceStatus.Normal),
            _ => (null, ServiceStatus.Error),
        };
    }

    public (Player? player, ServiceStatus status) Transfer(int playerid, int teamid)
    {
        var team = teamRepository.GetByID(teamid);
        if (team == null) return (null, ServiceStatus.Invaild);
        if (team.NumberOfPlayers >= Team.MaxSquadSize) return (null, ServiceStatus.TooMany);
        
        team.AddPlayerToTeam();
        teamRepository.UpdateTeam(team);

        var (player, status) = playerRepository.MoveToTeamById(teamid, playerid);
        return status switch
        {
            SaveStatus.Normal => (player, ServiceStatus.Normal),
            _ => (null, ServiceStatus.Error)
        };
    }
}