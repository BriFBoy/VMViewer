using Microsoft.Extensions.Logging;
using WMViewer.DataSync.Metrics;
using WMViewer.DataSync.Repository;
using WMViewer.DataSync.Validation;

namespace WMViewer.DataSync.Reader;

public class Processor(TeamRepository teamRepository, PlayerRepository playerRepository, 
    Validator validator, ILogger<Processor> logger, AffectedRowsMetric metrics, 
    DatasyncRepository datasyncRepository, CsvRecordReader csvRecordReader)
{

    private TeamRepository _teamRepository = teamRepository;
    private PlayerRepository _playerRepository = playerRepository;
    private Validator _validator = validator;
    private AffectedRowsMetric _metrics = metrics;
    private DatasyncRepository _datasyncRepository = datasyncRepository;
    private CsvRecordReader _csvRecordReader = csvRecordReader;

    public void ProcessRecord(PlayerMap playerMap)
    {


        if (string.IsNullOrEmpty(playerMap.TeamName))
        {
            return;
        }

        var (player, team) = _validator.CreateValidPlayerAndTeam(playerMap);
        if (player == null)
        {
            return;
        }
        
        if (team != null)
        {
            _teamRepository.UpdateTeam(team);
            _metrics.IncreaseTeamsmetric();
            logger.LogInformation("Updated Team, -> {TeamId}, {Name}, {NumberOfPlayers}", 
                team.TeamId, team.Name, team.NumberOfPlayers);
        }

        _playerRepository.AddPlayer(player);
        _metrics.IncreasePlayersmetric();
        logger.LogInformation("Updated Player, -> {PlayerId}, {Name}, {Age}, {Iscaptain}, {LastUpdate}, {TeamID}", 
            player.PlayerId, player.Name, player.Age, player.IsCaptain, player.LastUpdate, player.TeamId);
    }
    
    public DateTime? RecordLooper(string csvPath)
    {
        var currentLatest = _datasyncRepository.GetLatestUpdateTime();
        DateTime? lastUpdate = null;

        foreach (var record in _csvRecordReader.GetRecords(csvPath))
        {
            if (currentLatest != null && currentLatest >= record.LastUpdate)
            {
                _metrics.IncreaseSkippedsmetric();
                continue;
            }

            if (lastUpdate == null || lastUpdate < record.LastUpdate)
            {
                lastUpdate = record.LastUpdate;
            }


            ProcessRecord(record);
        }


        lastUpdate ??= currentLatest;
        return lastUpdate;
    }
}