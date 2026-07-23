namespace Shared.Model;

public class SyncOperation
{
    public SyncOperation(int? datasyncId, string csvName, DateTime dateOfSync, DateTime lastUpdate, string? status, string? errorMessage)
    {
        DatasyncId = datasyncId;
        CsvName = csvName;
        DateOfSync = dateOfSync;
        LastUpdate = lastUpdate;
        Status = status;
        ErrorMessage = errorMessage;
    }

    public int? DatasyncId { get; set; }
    public string CsvName { get; set; }
    public DateTime DateOfSync { get; set; }
    public DateTime LastUpdate { get; set; }
    public string? Status { get; set; }
    public string? ErrorMessage { get; set; }
}