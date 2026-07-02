using Dapper;
using Npgsql;
using VMViewer.Model;
using static VMViewer.Controllers.PlayerController;

namespace VMViewer.Repository;

public class PlayerRepository(NpgsqlConnection npgsqlConnection, ITeamRepository teamRepository) : IPlayerRepository
{
  readonly NpgsqlConnection npgsqlConnection = npgsqlConnection;
  readonly ITeamRepository teamRepository = teamRepository;


  public Player? GetById(int id)
  {
    const string sql = "SELECT * FROM players WHERE playerid=@Id";
    try
    {
      return npgsqlConnection.Query<Player>(sql,
 new { Id = id }).SingleOrDefault();
    }
    catch (Exception e)
    {

      Console.WriteLine(e);
      return null;
    }
  }
  public bool DoPlayerExistsWithName(string name)
  {
    return npgsqlConnection.QuerySingle<bool>("SELECT EXISTS(SELECT 1 FROM players WHERE name=@Name)", new { Name = name});
  }
  public bool DoPlayerExistsWithId(int id)
  {
    return npgsqlConnection.QuerySingle<bool>("SELECT EXISTS(SELECT 1 FROM players WHERE playerid=@Id)", new { Id = id});
  }
  public (Player?, SaveStatus) AddPlayer(Player player)
  {
    if (!teamRepository.DoTeamExistsWithId(player.TeamId)) return (null, SaveStatus.ErrorOccured);
    if (DoPlayerExistsWithName(player.Name)) return (null, SaveStatus.AlreadyExists);
    
    var i = npgsqlConnection.QuerySingleAsync<Player>("INSERT INTO players(age, teamid, Name) VALUES (@Age, @TeamId, @Name) RETURNING *",
      new { player.Age, player.TeamId, player.Name }).Result;

    return (i, SaveStatus.Created);

  }

  public SaveStatus DeletePlayer(int id)
  {
    if (!DoPlayerExistsWithId(id))
    {
      return SaveStatus.NoEntries;
    }

    var rows = npgsqlConnection.Execute("DELETE FROM players WHERE playerid=@Id", new { Id = id });
    return rows >= 1 ? SaveStatus.Deleted : SaveStatus.ErrorOccured;
  }

  public (List<Player>?, SaveStatus) GetAllPlayersInSquad(int teamId)
  {
    const string sql = @"SELECT * FROM players WHERE teamid=@Id";
    
    var squad = npgsqlConnection.Query<Player>(sql, new { Id= teamId});
    
    return squad.Count() == 0 ? (null, SaveStatus.NoEntries) : (squad.AsList(), SaveStatus.Normal);
  }


}