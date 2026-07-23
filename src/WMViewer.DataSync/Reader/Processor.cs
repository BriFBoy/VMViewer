using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using WMViewer.DataSync.Repository;
using WMViewer.DataSync.Validation;

namespace WMViewer.DataSync.Reader;

public class Processor(TeamRepository teamRepository, PlayerRepository playerRepository, Validator validator, ILogger<Processor> logger)
{

    private TeamRepository _teamRepository = teamRepository;
    private PlayerRepository _playerRepository = playerRepository;
    private Validator _validator = validator;

    public int ProcessRecord(PlayerMap playerMap)
    {
        var numberOfUpdates = 0;


        if (playerMap.TeamName.IsNullOrEmpty())
        {
            return 0;
        }

        var (player, team, affected) = _validator.CreateValidPlayerAndTeam(playerMap);
        if (player == null)
        {
            return 0;
        }

        numberOfUpdates += affected;

        if (team != null)
        {
            numberOfUpdates += _teamRepository.UpdateTeam(team);
            logger.LogInformation("Updated Team, -> {TeamId}, {Name}, {NumberOfPlayers}", 
                team.TeamId, team.TeamId, team.NumberOfPlayers);
        }

        numberOfUpdates += _playerRepository.AddPlayer(player);
        logger.LogInformation("Updated Player, -> {PlayerId}, {Name}, {Age}, {Iscaptain}, {LastUpdate}, {TeamID}", 
            player.PlayerId, player.Name, player.Age, player.IsCaptain, player.LastUpdate, player.TeamId);
        
        return numberOfUpdates;
    }
}