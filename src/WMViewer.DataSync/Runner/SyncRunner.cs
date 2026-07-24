using System.Data;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Model;
using WMViewer.DataSync.Metrics;
using WMViewer.DataSync.Reader;
using WMViewer.DataSync.Repository;
using CsvReader = CsvHelper.CsvReader;

namespace WMViewer.DataSync.Runner;

public class SyncRunner
{
    private IDbConnection _connection;
    private ILogger<SyncRunner> _logger;
    private DatasyncRepository _datasyncRepository;
    private Processor _processor;

    public SyncRunner(IDbConnection connection, Processor processor,
        ILogger<SyncRunner> logger, DatasyncRepository datasyncRepository)
    {
        _connection = connection;
        _datasyncRepository = datasyncRepository;
        _logger = logger;
        _processor = processor;
    }

    public void RunSync(string csvPath)
    {
        _connection.Open();
        _logger.LogInformation("Beginning sync");
        using (var transaction = _connection.BeginTransaction())
        {
            try
            {
                var lastUpdate = _processor.RecordLooper(csvPath);
                if (lastUpdate == null)
                {
                    throw new ArgumentNullException("lastUpdate");
                }

                var syncoperation = new SyncOperation(null, csvPath, DateTime.Now, lastUpdate.Value, "success", null);

                _datasyncRepository.AddSyncLog(syncoperation);
                _logger.LogInformation("Commiting Transaction");
                transaction.Commit();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while running sync");
                transaction.Rollback();
                var lastUpdate = _datasyncRepository.GetLatestUpdateTime();
                var syncoperation = new SyncOperation(null, csvPath, DateTime.Now, lastUpdate.Value, "failed", e.Message);
                _datasyncRepository.AddSyncLog(syncoperation);
                throw;
            }
        }

        _connection.Close();
        _logger.LogInformation("Sync done");
    }
}