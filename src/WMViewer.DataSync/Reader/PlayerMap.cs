using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Shared.Model;

namespace WMViewer.DataSync.Reader;

public record PlayerMap
{

    
    [Name("long_name")]
    public string Name { get; set; }
    [Name("age")]
    public int Age { get; set; }
    [Name("club_name")]
    public string TeamName { get; set; }
    [Name("player_id")]
    public int? PlayerId { get; set; }
    [Ignore]
    public bool IsCaptain { get; set; } = false;
    
    [Name("fifa_update_date")]
    public DateTime LastUpdate { get; set; }



};