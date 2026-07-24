using CsvHelper;
using CsvHelper.Configuration;

namespace WMViewer.DataSync.Reader;

public class CsvRecordReader(CsvConfiguration config)

{
    public IEnumerable<PlayerMap> GetRecords(string csvPath)
    {
        using var reader = new StreamReader(csvPath!);
        using var csv = new CsvReader(reader, config);

        foreach (var record in csv.GetRecords<PlayerMap>())
        {
            yield return record;
        }
    }
    
}