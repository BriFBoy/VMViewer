using System.Diagnostics.Metrics;

namespace WMViewer.DataSync.Metrics;

public class AffectedRowsMetric
{
    private readonly Counter<int> _affectedPlayersCounter;
    private readonly Counter<int> _affectedTeamsCounter;
    private readonly Counter<int> _rowsSkiped;

    public AffectedRowsMetric(IMeterFactory factory)
    {
        Meter meter = factory.Create("wmviewer.dataSync");
        _affectedPlayersCounter = meter.CreateCounter<int>("affectedplayers");
        _affectedTeamsCounter = meter.CreateCounter<int>("affectedteams");
        _rowsSkiped = meter.CreateCounter<int>("skippedrows");
    }

    public void IncreasePlayersmetric()
    {
        _affectedPlayersCounter.Add(1);
    }
    public void IncreaseTeamsmetric()
    {
        _affectedTeamsCounter.Add(1);
    }
    public void IncreaseSkippedsmetric()
    {
        _rowsSkiped.Add(1);
    }
    /*
    public void GetTeamMetricCount()
    {
        _affectedTeamsCounter.
    }
    */
}