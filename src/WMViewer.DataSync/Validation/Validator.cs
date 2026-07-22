using System.Data;
using Microsoft.IdentityModel.Tokens;
using Shared.Model;
using WMViewer.DataSync.Reader;
using WMViewer.DataSync.Repository;

namespace WMViewer.DataSync.Validation;

public class Validator(TeamRepository teamRepository)
{
    public (Player?, Team?) CreateValidPlayerAndTeam(PlayerMap mapper)
    {
        if (!teamRepository.DoTeamExistsWithName(mapper.TeamName))
        {
            var team = new Team(null, mapper.TeamName, 1, null);
            var saved = teamRepository.SaveTeam(team);
            if (saved is null)
            {
                return (null, null);
            }

            var player = new Player(null, mapper.Name, mapper.Age, saved.Value, DateTime.Now, mapper.IsCaptain);
            team.AddPlayerToTeam();
            return (player, null);
        }

        var team1 = teamRepository.GetTeamByName(mapper.TeamName);
        if (team1?.TeamId is null)
        {
            return (null, null);
        }

        var player1 = new Player(null, mapper.Name, mapper.Age, team1.TeamId.Value, DateTime.Now,
            mapper.IsCaptain);
        team1.AddPlayerToTeam();
        return (player1, team1);
    }
}