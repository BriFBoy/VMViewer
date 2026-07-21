using Shared.Model;

namespace VMViewer.Service;

public interface ITeamService
{
    public Team? GetTeam(int teamid);

    public (Team?, ServiceStatus) AddTeam(string? name);

    public ServiceStatus DeleteTeam(int teamid);
    (Player? player, ServiceStatus status) MakeCaptain(int teamid, int playerid);
}