using Npgsql;
using VMViewer.Model;
using VMViewer.Repository;

namespace VMViewer.Service;

public class PlayerService(IPlayerRepository playerRepository, ITeamRepository teamRepository, ILogger<PlayerService> logger)
    : IPlayerService
{
    public (Player?, ServiceStatus) GetPlayer(int id)
    {
        if (id <= 0) return (null, ServiceStatus.Invaild);

        var player = playerRepository.GetById(id);
        return player is not null ? (player, ServiceStatus.Normal) : (null, ServiceStatus.NotFound);
    }

    public (Player?, ServiceStatus) AddPlayer(string name, int age, int teamid)
    {
        try
        {
            if (!teamRepository.DoTeamExistsWithId(teamid)) return (null, ServiceStatus.Invaild);
            if (playerRepository.DoPlayerExistsWithName(name)) return (null, ServiceStatus.Exists);

            var (players, getstatus) = playerRepository.GetAllPlayersInSquad(teamid);
            if (getstatus != SaveStatus.Normal && getstatus != SaveStatus.NoEntries) return (null, ServiceStatus.Error);

            if (players is not null)
            {
                if (players.Count >= Team.MAXSQUADSIZE) return (null, ServiceStatus.TooMany);
            }


            var newPlayer = new Player(null, name, age, teamid, false);
            var (player, status) = playerRepository.AddPlayer(newPlayer);
            return status switch
            {
                SaveStatus.AlreadyExists => (null, ServiceStatus.Exists),
                SaveStatus.ErrorOccured => (null, ServiceStatus.Error),
                SaveStatus.Created => (player, ServiceStatus.Normal),
                _ => (null, ServiceStatus.Error)
            };
        }
        catch (NpgsqlException e)
        {
            logger.LogError("Database failed when saving player; Error: {e}", e);
            return (null, ServiceStatus.Error);
        }
    }

    public ServiceStatus DeletePlayer(int playerid)
    {
        if (playerid <= 0)
        {
            return ServiceStatus.Invaild;
        }

        var res = playerRepository.DeletePlayer(playerid);
        return res switch
        {
            SaveStatus.NoEntries => ServiceStatus.NotFound,
            SaveStatus.Deleted => ServiceStatus.Normal,
            _ => ServiceStatus.Error
        };
    }
}