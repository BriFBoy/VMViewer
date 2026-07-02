using VMViewer.Model;

namespace VMViewer.Service;

 public interface ISquadService
{
 public (List<Player>?, ServiceStatus) GetSquad(int teamid);
}