namespace Shared.Model;

public class Team(int? teamid, string name, int numberofplayers, int? captain)
{

  public string Name { get; } = name;
  public int? TeamId { get; } = teamid;
  public int NumberOfPlayers = numberofplayers;
  public int? Captain { get; set;  } = captain;
  
  public const int MAXSQUADSIZE = 25;

  public void AddPlayerToTeam()
  {
    NumberOfPlayers++;
  }
  public void RemovePlayerToTeam()
  {
    NumberOfPlayers--;
  }
  public int GetNumberOfPlayers()
  {
    return NumberOfPlayers;
  }
}
