using Dapper;
using Npgsql;
using VMViewer.Model;

namespace VMViewer.Repository;

public class TeamRepository(NpgsqlConnection npgsqlConnection) : ITeamRepository
{
    readonly NpgsqlConnection npgsqlConnection = npgsqlConnection;

    public Team? GetByID(int Id)
    {
        try
        {
            return npgsqlConnection.QuerySingle<Team>("SELECT * FROM teams WHERE teamid=@Id", new { Id });
        }
        catch (Exception)
        {
            return null;
        }
    }

    public bool DoTeamExistsWithId(int Id)
    {
        return Id > 0 &&
               npgsqlConnection.QuerySingle<bool>("SELECT EXISTS(SELECT 1 FROM teams WHERE teamid=@Id)", new { Id });
    }

    public bool DoTeamExistsWithName(string Name)
    {
        return npgsqlConnection.QuerySingle<bool>("SELECT EXISTS(SELECT 1 FROM teams WHERE name=@Name)", new { Name });
    }

    public (Team?, SaveStatus) SaveTeam(Team team)
    {
        if (DoTeamExistsWithName(team.Name)) return (null, SaveStatus.AlreadyExists);

        var result = npgsqlConnection
            .QuerySingleAsync<Team>("INSERT INTO teams (name, numberofplayers) VALUES (@Name, @numberofplayers) RETURNING *", new { team.Name, team.NumberOfPlayers }).Result;
        return (result, SaveStatus.Created);
    }

    public SaveStatus DeleteTeam(int Id)
    {
        if (!DoTeamExistsWithId(Id)) return SaveStatus.NoEntries;

        IEnumerable<int> playerIds =
            npgsqlConnection.Query<int>("SELECT playerid FROM players WHERE teamid=@Id", new { Id });
        foreach (var playerid in playerIds)
        {
            npgsqlConnection.Execute("DELETE FROM players WHERE playerid=@Id", new { Id = playerid });
        }

        var res = npgsqlConnection.Execute("DELETE FROM teams WHERE teamid=@Id", new { Id });
        return res >= 1 ? SaveStatus.Deleted : SaveStatus.ErrorOccured;
    }

    public SaveStatus UpdateTeam(Team team)
    {
        const string sql = "UPDATE teams SET numberofplayers=@numberofplayers, name=@name WHERE teamid=@teamid";

        var rowsupdated = npgsqlConnection.Execute(sql, new { team.NumberOfPlayers, team.Name, team.TeamId });
        return (rowsupdated >= 0) ? SaveStatus.Normal : SaveStatus.ErrorOccured;
    }
}