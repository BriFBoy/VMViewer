namespace VMViewer.Model;

public class Team(int? teamid, string name)
{

  public string Name { get; } = name;
  public int? TeamId { get; } = teamid;
}
