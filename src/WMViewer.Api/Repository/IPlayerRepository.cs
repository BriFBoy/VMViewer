using Shared.Model;

namespace VMViewer.Repository;

public interface IPlayerRepository
{
    public Player? GetById(int id);

    public bool DoPlayerExistsWithName(string name);
    public bool DoPlayerExistsWithId(int id);

    public (Player?, SaveStatus) AddPlayer(Player player);
    public SaveStatus DeletePlayer(int id);

    public (List<Player>?, SaveStatus) GetAllPlayersInSquad(int teamId);
    (Player?, SaveStatus) MoveToTeamById(int teamid, int playerid);

    SaveStatus UpdatePlayer(Player player);
}