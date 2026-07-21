using Microsoft.IdentityModel.Tokens;
using Shared.Model;
using WMViewer.DataSync.Reader;
using WMViewer.DataSync.Repository;

namespace WMViewer.DataSync.Validation;

public class Validator(TeamRepository teamRepository)
{

    public Player? CreatePlayer(PlayerMap mapper)
    {
        if (!teamRepository.DoTeamExistsWithName(mapper.TeamName))
        {
            var team = new Team(null, mapper.TeamName, 0, null);
            var saved = teamRepository.SaveTeam(team);
            if (saved is null)
            {
                return null;
            }
            
            var player = new Player(null, mapper.Name, mapper.Age, saved.Value, mapper.IsCaptain);
            return player;
        }

        var team1 = teamRepository.GetTeamByName(mapper.TeamName);
        if (team1 is null)
        {
            return null;
        }
        var player1 = new Player(null, mapper.Name, mapper.Age, team1.TeamId.Value, mapper.IsCaptain);
        return player1;
        
    }
    
}