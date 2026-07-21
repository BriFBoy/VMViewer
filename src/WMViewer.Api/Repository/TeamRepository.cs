using Dapper;
using Npgsql;
using Shared.Model;

namespace VMViewer.Repository;

public class TeamRepository(NpgsqlConnection npgsqlConnection) : ITeamRepository
{
    readonly NpgsqlConnection _npgsqlConnection = npgsqlConnection;

    public Team? GetByID(int id)
    {
        try
        {
            return _npgsqlConnection.QuerySingle<Team>("SELECT * FROM teams WHERE teamid=@Id", new { Id = id });
        }
        catch (Exception)
        {
            return null;
        }
    }

    public bool DoTeamExistsWithId(int id)
    {
        return id > 0 &&
               _npgsqlConnection.QuerySingle<bool>("SELECT EXISTS(SELECT 1 FROM teams WHERE teamid=@Id)", new { Id = id });
    }

    public bool DoTeamExistsWithName(string name)
    {
        return _npgsqlConnection.QuerySingle<bool>("SELECT EXISTS(SELECT 1 FROM teams WHERE name=@Name)", new { Name = name });
    }

    public (Team?, SaveStatus) SaveTeam(Team team)
    {
        if (DoTeamExistsWithName(team.Name)) return (null, SaveStatus.AlreadyExists);

        var result = _npgsqlConnection
            .QuerySingleAsync<Team>("INSERT INTO teams (name, numberofplayers) VALUES (@Name, @numberofplayers) RETURNING *", new { team.Name, team.NumberOfPlayers }).Result;
        return (result, SaveStatus.Created);
    }

    public SaveStatus DeleteTeam(int id)
    {
        if (!DoTeamExistsWithId(id)) return SaveStatus.NoEntries;

        var playerIds =
            _npgsqlConnection.Query<int>("SELECT playerid FROM players WHERE teamid=@Id", new { Id = id });
        foreach (var playerid in playerIds)
        {
            _npgsqlConnection.Execute("DELETE FROM players WHERE playerid=@Id", new { Id = playerid });
        }

        var res = _npgsqlConnection.Execute("DELETE FROM teams WHERE teamid=@Id", new { Id = id });
        return res >= 1 ? SaveStatus.Deleted : SaveStatus.ErrorOccured;
    }

    public SaveStatus UpdateTeam(Team team)
    {
        const string sql = "UPDATE teams SET numberofplayers=@numberofplayers, name=@name, captain=@captain WHERE teamid=@teamid";

        var rowsupdated = _npgsqlConnection.Execute(sql, new { team.NumberOfPlayers, team.Name, team.TeamId, team.Captain });
        return (rowsupdated >= 0) ? SaveStatus.Normal : SaveStatus.ErrorOccured;
    }
}