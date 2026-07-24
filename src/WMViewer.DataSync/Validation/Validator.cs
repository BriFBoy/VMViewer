using Microsoft.Extensions.Logging;
using Shared.Model;
using WMViewer.DataSync.Metrics;
using WMViewer.DataSync.Reader;
using WMViewer.DataSync.Repository;

namespace WMViewer.DataSync.Validation;

public class Validator(TeamRepository teamRepository, AffectedRowsMetric metrics)
{

    public (Player?, Team?) CreateValidPlayerAndTeam(PlayerMap mapper)
    {
        if (!teamRepository.DoTeamExistsWithName(mapper.TeamName))
        {
            var team = new Team(null, mapper.TeamName, 0, null);
            var (teamid, affected ) = teamRepository.SaveTeam(team);
            metrics.IncreaseTeamsmetric();
            if (teamid == null)
            {
                return (null, null);
            }

            var player = new Player(null, mapper.Name, mapper.Age, teamid.Value, mapper.LastUpdate, mapper.IsCaptain);
            return (player, null);
        }

        var team1 = teamRepository.GetTeamByName(mapper.TeamName);
        if (team1?.TeamId is null)
        {
            return (null, null);
        }

        var player1 = new Player(null, mapper.Name, mapper.Age, team1.TeamId.Value, mapper.LastUpdate,
            mapper.IsCaptain);
        team1.AddPlayerToTeam();
        return (player1, team1);
    }
}