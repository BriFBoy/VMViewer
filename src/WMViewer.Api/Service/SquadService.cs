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
        if (team.NumberOfPlayers >= Team.MAXSQUADSIZE) return (null, ServiceStatus.TooMany);
        
        var (player, status) = playerRepository.MoveToTeamById(teamid, playerid);
        if (player == null) return (null, ServiceStatus.Error);
        
        team.AddPlayerToTeam();
        teamRepository.UpdateTeam(team);
        
        var oldteam = teamRepository.GetByID(player.TeamId);
        if (oldteam == null) return (null, ServiceStatus.Error);
        
        oldteam.RemovePlayerToTeam();
        teamRepository.UpdateTeam(oldteam);
        
        return status switch
        {
            SaveStatus.Normal => (player, ServiceStatus.Normal),
            _ => (null, ServiceStatus.Error)
        };
    }
}