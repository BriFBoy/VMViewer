using System.Data;
using Dapper;
using Shared.Model;

namespace WMViewer.DataSync.Repository;

public class DatasyncRepository(IDbConnection connection)
{
    private IDbConnection _connection = connection;


    public int AddSyncLog(SyncOperation dataSync)
    {
       return _connection.Execute("""
                                  INSERT INTO datasync ( csv_name, date_of_sync, last_update, status, error_message)
                                              VALUES (@csv_name, @date_of_sync, @last_update, @status, @error_message)
                                  """,
           new { csv_name = dataSync.CsvName, date_of_sync = dataSync.DateOfSync, last_update = dataSync.LastUpdate,
               status = dataSync.Status,
               error_message = dataSync.ErrorMessage});
    }

    public DateTime? GetLatestUpdateTime()
    {
        return _connection.QuerySingle<DateTime?>("SELECT MAX(last_update) FROM datasync");
    }
}