using Dapper;
using Npgsql;
using VMViewer.Model;

namespace VMViewer.Repository;

public class PlayerRepository(NpgsqlConnection npgsqlConnection, ITeamRepository teamRepository) : IPlayerRepository
{
  readonly NpgsqlConnection _npgsqlConnection = npgsqlConnection;
  readonly ITeamRepository _teamRepository = teamRepository;


  public Player? GetById(int id)
  {
    const string sql = "SELECT * FROM players WHERE playerid=@Id";
    try
    {
      return _npgsqlConnection.Query<Player>(sql,
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
    return _npgsqlConnection.QuerySingle<bool>("SELECT EXISTS(SELECT 1 FROM players WHERE name=@Name)", new { Name = name});
  }
  public bool DoPlayerExistsWithId(int id)
  {
    return _npgsqlConnection.QuerySingle<bool>("SELECT EXISTS(SELECT 1 FROM players WHERE playerid=@Id)", new { Id = id});
  }
  public (Player?, SaveStatus) AddPlayer(Player player)
  {
    if (!_teamRepository.DoTeamExistsWithId(player.TeamId)) return (null, SaveStatus.ErrorOccured);
    if (DoPlayerExistsWithName(player.Name)) return (null, SaveStatus.AlreadyExists);
    
    var i = _npgsqlConnection.QuerySingleAsync<Player>("INSERT INTO players(age, teamid, Name) VALUES (@Age, @TeamId, @Name) RETURNING *",
      new { player.Age, player.TeamId, player.Name }).Result;

    return (i, SaveStatus.Created);

  }

  public SaveStatus DeletePlayer(int id)
  {
    if (!DoPlayerExistsWithId(id))
    {
      return SaveStatus.NoEntries;
    }

    var rows = _npgsqlConnection.Execute("DELETE FROM players WHERE playerid=@Id", new { Id = id });
    return rows >= 1 ? SaveStatus.Deleted : SaveStatus.ErrorOccured;
  }

  public (List<Player>?, SaveStatus) GetAllPlayersInSquad(int teamId)
  {
    const string sql = @"SELECT * FROM players WHERE teamid=@Id";
    
    var squad = _npgsqlConnection.Query<Player>(sql, new { Id= teamId});
    
    return squad.Count() == 0 ? (null, SaveStatus.NoEntries) : (squad.AsList(), SaveStatus.Normal);
  }

  public (Player?, SaveStatus) MoveToTeamById(int teamid, int playerid)
  {
    const string sql = "UPDATE players SET teamid=@teamid WHERE playerid=@playerid";

    var rowsupdates = _npgsqlConnection.Execute(sql, new {teamid, playerid});
    if (rowsupdates <= 0) return (null, SaveStatus.ErrorOccured);

    var updatedplayer = _npgsqlConnection.QuerySingleOrDefault<Player>("SELECT * FROM players WHERE playerid=@playerid", new { playerid });
    return updatedplayer == null ? (null, SaveStatus.ErrorOccured) : (updatedplayer, SaveStatus.Normal);
  }

  public SaveStatus UpdatePlayer(Player player)
  {
    const string sql = "UPDATE players SET name=@name, age=@age, teamid=@teamid, iscaptain=@iscaptain WHERE playerid=@playerid";

    var rowsupdated = _npgsqlConnection.Execute(sql, new { player.Name, player.Age, player.TeamId, player.IsCaptain, player.PlayerId });
    return (rowsupdated <= 0)  ? SaveStatus.ErrorOccured : SaveStatus.Normal;
  }
}