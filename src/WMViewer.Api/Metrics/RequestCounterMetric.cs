using System.Diagnostics.Metrics;

namespace VMViewer.Metrics;

public interface IRequestCounterMetric
{
    void Increase();
}
public class RequestCounterMetric: IRequestCounterMetric
{
    private readonly Counter<int> _counter;

    public RequestCounterMetric(IMeterFactory factory)
    {
        var meter = factory.Create("wmviewer.api");
        _counter = meter.CreateCounter<int>("wmviewer.api.count");
    }

    public void Increase ()
    {
        _counter.Add(1);
    }
}