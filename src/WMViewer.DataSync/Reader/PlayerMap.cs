using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration.Attributes;

namespace WMViewer.DataSync.Reader;

public record PlayerMap
{

    
    [Name("long_name")]
    [Required]
    public string Name { get; set; }
    [Name("age")]
    [Required]
    public int Age { get; set; }
    [Name("club_name")]
    [Required]
    public string TeamName { get; set; }
    [Name("player_id")]
    public int? PlayerId { get; set; }
    [Ignore]
    public bool IsCaptain { get; set; } = false;
    
    [Name("fifa_update_date")]
    [Required]
    public DateTime LastUpdate { get; set; }



};