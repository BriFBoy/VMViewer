using VMViewer.Model;

namespace VMViewer.Service;

 public interface ISquadService
{
 public (List<Player>?, ServiceStatus) GetSquad(int teamid);
 (Player? player, ServiceStatus status) Transfer(int playerid, int teamid);
}