using System.Data;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Shared.Model;
using WMViewer.DataSync.Reader;
using WMViewer.DataSync.Repository;

namespace WMViewer.DataSync.Validation;

public class Validator(TeamRepository teamRepository, ILogger<Validator> logger)
{
    // Return Player, Team and an affected rows
    // The Affected rows is only used when a new team is inserted into the db
    public (Player?, Team?, int) CreateValidPlayerAndTeam(PlayerMap mapper)
    {
        if (!teamRepository.DoTeamExistsWithName(mapper.TeamName))
        {
            var team = new Team(null, mapper.TeamName, 0, null);
            var (teamid, affected ) = teamRepository.SaveTeam(team);
            if (teamid == null)
            {
                return (null, null, 0);
            }

            var player = new Player(null, mapper.Name, mapper.Age, teamid.Value, mapper.LastUpdate, mapper.IsCaptain);
            return (player, null, affected);
        }

        var team1 = teamRepository.GetTeamByName(mapper.TeamName);
        if (team1?.TeamId is null)
        {
            return (null, null, 0);
        }

        var player1 = new Player(null, mapper.Name, mapper.Age, team1.TeamId.Value, mapper.LastUpdate,
            mapper.IsCaptain);
        team1.AddPlayerToTeam();
        return (player1, team1, 0);
    }
}