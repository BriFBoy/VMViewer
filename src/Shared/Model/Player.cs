namespace Shared.Model;

public class Player
{
    public Player(int? playerId, string name, int age, int teamId, DateTime lastUpdate, bool isCaptain)
    {
        Name = name;
        Age = age;
        TeamId = teamId;
        PlayerId = playerId;
        IsCaptain = isCaptain;
        LastUpdate = lastUpdate;
    }
    public Player() {}


    public string Name { get; set; }
    public int Age { get; set; }
    public int TeamId { get; set; }
    public int? PlayerId { get; set; }
    public bool IsCaptain { get; set; }

    public DateTime LastUpdate { get; set; }
}