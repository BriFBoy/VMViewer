using VMViewer.Model;
using VMViewer.Repository;

namespace VMViewer.Service;

public class TeamService(ITeamRepository teamRepository): ITeamService
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
    
        var (team, status) =  teamRepository.SaveTeam(new Team(null, name));
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
}