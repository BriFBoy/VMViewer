namespace VMViewer.Model;

public class Player(int? playerId, string name, int age, int teamId, bool iscaptain)
{
  public string Name { get; set; } = name;
  public int Age { get; set; } = age;
  public int TeamId { get; set; } = teamId;
  public int? PlayerId { get; set; } = playerId;
  public bool IsCaptain { get; set; } = iscaptain;
}
