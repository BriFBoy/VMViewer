using VMViewer.Model;
using VMViewer.Repository;

namespace VMViewer.Service;

public class SquadService(IPlayerRepository playerRepository): ISquadService
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
}