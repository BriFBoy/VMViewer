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
    public DateTime? GetLastUpdateByName(string name)
    {
        return connection.QuerySingleOrDefault<DateTime?>("SELECT last_update FROM players WHERE name=@Name", new { Name = name });
    }

    public int AddPlayer(Player player)
    {
        if (!teamRepository.DoTeamExistsWithId(player.TeamId)) return 0;

        if (DoPlayerExistsWithName(player.Name))
        {
            return connection.Execute(
                "UPDATE players SET age=@Age, teamid=@TeamId, last_update=@LastUpdate, iscaptain=@IsCaptain WHERE name=@Name",
                new { player.Age, player.TeamId, player.LastUpdate, player.IsCaptain, player.Name });
             
        }

       return connection.Execute(
            "INSERT INTO players(age, teamid, name, last_update, iscaptain) VALUES (@Age, @TeamId, @Name, @LastUpdate, @IsCaptain)",
            new { player.Age, player.TeamId, player.Name, player.LastUpdate, player.IsCaptain });

       
    }

}