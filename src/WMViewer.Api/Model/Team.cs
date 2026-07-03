namespace VMViewer.Model;

public class Team(int? teamid, string name, int numberofplayers)
{

  public string Name { get; } = name;
  public int? TeamId { get; } = teamid;
  public int NumberOfPlayers = numberofplayers;
  
  public const int MaxSquadSize = 25;

  public void AddPlayerToTeam()
  {
    NumberOfPlayers++;
  }
  public int GetNumberOfPlayers()
  {
    return NumberOfPlayers;
  }
}
