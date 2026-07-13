using VMViewer.Model;
using VMViewer.Repository;

namespace VMViewer.Service;

public class TeamService(ITeamRepository teamRepository, IPlayerRepository playerRepository): ITeamService
{
    
    
    public Team? GetTeam(int teamid)
    {
        return teamid <= 0 ? null : teamRepository.GetByID(teamid);
    }

    public (Team?, ServiceStatus) AddTeam(string? name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return (null, ServiceStatus.Invaild);
        }
    
        var (team, status) =  teamRepository.SaveTeam(new Team(null, name, 0, null));
        return status switch
        {
            SaveStatus.AlreadyExists => (null, ServiceStatus.Exists),
            SaveStatus.Created => (team, ServiceStatus.Normal),
            _ => (null, ServiceStatus.Error)
        };
    }

    public ServiceStatus DeleteTeam(int teamid)
    {
        if (teamid <= 0)
        {
            return ServiceStatus.Invaild;
        }

        return teamRepository.DeleteTeam(teamid) switch
        {
            SaveStatus.NoEntries => ServiceStatus.NotFound,
            SaveStatus.Deleted => ServiceStatus.Normal,
            _ => ServiceStatus.Error,
        };
    }

    public (Player? player, ServiceStatus status) MakeCaptain(int teamid, int playerid)
    {
        if (teamid <= 0 || playerid <= 0) return (null, ServiceStatus.Invaild);
        
        var player = playerRepository.GetById(playerid);
        if (player == null) return (null, ServiceStatus.Invaild);

        var team = teamRepository.GetByID(teamid);
        if (team == null) return (null, ServiceStatus.Invaild);

        team.Captain = playerid;
        player.IsCaptain = true;
        if (teamRepository.UpdateTeam(team) == SaveStatus.ErrorOccured ||
            playerRepository.UpdatePlayer(player) == SaveStatus.ErrorOccured)
            return (null, ServiceStatus.Error);

        return (player, ServiceStatus.Normal);
    }
}