using System.Data;
using Dapper;
using Shared.Model;

namespace WMViewer.DataSync.Repository;

public class TeamRepository(IDbConnection connection)
{
    public bool DoTeamExistsWithId(int id)
    {
        return id > 0 &&
               connection.QuerySingle<bool>("SELECT EXISTS(SELECT 1 FROM teams WHERE teamid=@Id)", new { Id = id });
    }

    public bool DoTeamExistsWithName(string name)
    {
        return connection.QuerySingle<bool>("SELECT EXISTS(SELECT 1 FROM teams WHERE name=@Name)", new { Name = name });
    }

    public int? SaveTeam(Team team)
    {
        try
        {
            if (DoTeamExistsWithName(team.Name)) return null;
            var result = connection
                .QuerySingleAsync<Team>(
                    "INSERT INTO teams (name, numberofplayers) VALUES (@Name, @numberofplayers) RETURNING *",
                    new { team.Name, team.NumberOfPlayers }).Result;
            return result.TeamId;
        }
        catch (AggregateException e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public Team? GetTeamByName(string name)
    {
        try
        {
            return connection.QuerySingle<Team>("SELECT * FROM teams WHERE name=@Name", new { Name = name });
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to get team by name: " + name);
            Console.WriteLine(e);
            return null;
        }
       
    }
}