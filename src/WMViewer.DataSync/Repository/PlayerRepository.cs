using System.Data;
using System.Runtime.InteropServices.Marshalling;
using Dapper;
using Shared.Model;

namespace WMViewer.DataSync.Repository;

public class PlayerRepository(TeamRepository teamRepository, IDbConnection connection)
{
    public bool DoPlayerExistsWithName(string name)
    {
        return connection.QuerySingle<bool>("SELECT EXISTS(SELECT 1 FROM players WHERE name=@Name)", new { Name = name});
    }
    public bool AddPlayer(Player player)
    {
        if (!teamRepository.DoTeamExistsWithId(player.TeamId)) return false;
        if (DoPlayerExistsWithName(player.Name)) return false;
    
        var i = connection.QuerySingleAsync<Player>("INSERT INTO players(age, teamid, Name) VALUES (@Age, @TeamId, @Name) RETURNING *",
            new { player.Age, player.TeamId, player.Name }).Result;

        return true;

    }
    
}