using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VMViewer.Controllers;
using VMViewer.Model;
using VMViewer.Repository;

namespace VMViewer.Service;

public class PlayerService(IPlayerRepository playerRepository, ITeamRepository teamRepository): IPlayerService
{
    private const int MaxSquadSize = 25;
    
    public (Player?, ServiceStatus) GetPlayer(int id)
    {
        if (id <= 0) return (null, ServiceStatus.Invaild);

        var player = playerRepository.GetById(id);
        return player is not null ? (player, ServiceStatus.Normal) : (null, ServiceStatus.NotFound);
    }

    public (Player?, ServiceStatus) AddPlayer(string name, int age, int teamid)
    {
        
        if (!teamRepository.DoTeamExistsWithId(teamid)) return (null, ServiceStatus.Invaild);
        if (playerRepository.DoPlayerExistsWithName(name)) return (null, ServiceStatus.Exists);
        
        var (players, getstatus) = playerRepository.GetAllPlayersInSquad(teamid);
        if (getstatus != SaveStatus.Normal || players == null) return (null, ServiceStatus.Error);

        if ( players.Count >= MaxSquadSize) return (null, ServiceStatus.TooMany);
        
        
        var newPlayer = new Player(null, name, age, teamid);
        var (player, status) = playerRepository.AddPlayer(newPlayer);

        return status switch
        {
            SaveStatus.AlreadyExists => (null, ServiceStatus.Exists),
            SaveStatus.ErrorOccured => (null, ServiceStatus.Error),
            SaveStatus.Created => (player, ServiceStatus.Normal),
            _ => (null, ServiceStatus.Error)
        };
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
            SaveStatus.NoEntries =>  ServiceStatus.NotFound,
            SaveStatus.Deleted => ServiceStatus.Normal,
            _ => ServiceStatus.Error
        };
    }
}