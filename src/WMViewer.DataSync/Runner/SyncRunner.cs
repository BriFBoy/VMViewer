using System.Data;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Model;
using WMViewer.DataSync.Reader;
using WMViewer.DataSync.Repository;
using CsvReader = CsvHelper.CsvReader;

namespace WMViewer.DataSync.Runner;

public class SyncRunner
{
    private CsvConfiguration _csvHelperConfig;
    private IDbConnection _connection;
    private Processor _processor;
    private ILogger<SyncRunner> _logger;
    private DatasyncRepository _datasyncRepository;

    public SyncRunner(CsvConfiguration csvHelperConfig, IDbConnection connection, Processor processor, ILogger<SyncRunner> logger, DatasyncRepository datasyncRepository)
    {
        _csvHelperConfig = csvHelperConfig;
        _connection = connection;
        _processor = processor;
        _logger = logger;
        _datasyncRepository = datasyncRepository;
    }
    public void RunSync(string? csvPath)
    {
        using var reader = new StreamReader(csvPath!);
        using var csv = new CsvReader(reader, _csvHelperConfig);

        using var records = csv.GetRecords<PlayerMap>().GetEnumerator();

        _connection.Open();
        _logger.LogInformation("Beginning sync");
        using (var transaction = _connection.BeginTransaction())
        {
            try
            {
                var skipedRecords = 0;
                var affectedRows = 0;
                var currentLatest = _datasyncRepository.GetLatestUpdateTime();
                DateTime? lastUpdate = null;
      
                while (records.MoveNext())
                {
               
                    if (currentLatest != null && currentLatest >= records.Current.LastUpdate)
                    {
                        skipedRecords++;
                        continue;
                    }
                    
                    if (lastUpdate == null || lastUpdate < records.Current.LastUpdate)
                    {
                        lastUpdate = records.Current.LastUpdate;
                    }


                    affectedRows += _processor.ProcessRecord(records.Current);
                }

                lastUpdate ??= currentLatest;


                var syncoperation = new SyncOperation(null, csvPath!, DateTime.Now, lastUpdate.Value, "success", null);

                _datasyncRepository.AddSyncLog(syncoperation);
                _logger.LogInformation("Commiting Transaction");
                _logger.LogInformation("{rows} Affected; Skipped {skipped}", affectedRows, skipedRecords);
                transaction.Commit();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while running sync");
                transaction.Rollback();
                throw;
            }
        }

        _connection.Close();
        _logger.LogInformation("Sync done");
    }
}