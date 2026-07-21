using Shared.Model;

namespace VMViewer.Service;

public interface IPlayerService
{
    public (Player?, ServiceStatus) GetPlayer(int id);

    public (Player?, ServiceStatus) AddPlayer(string name, int age, int teamid);

    public ServiceStatus DeletePlayer(int playerid);
}